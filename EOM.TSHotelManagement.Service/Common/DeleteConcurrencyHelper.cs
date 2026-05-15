using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using EOM.TSHotelManagement.Contract;
using Microsoft.AspNetCore.Http;

namespace EOM.TSHotelManagement.Service
{
    public class DeleteConcurrencyHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public DeleteConcurrencyHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static List<int> GetDeleteIds(DeleteDto deleteDto)
        {
            return deleteDto?.DelIds?
                .Select(x => x.Id)
                .Distinct()
                .ToList() ?? new List<int>();
        }

        public static bool HasDeleteConflict<TEntity>(
            DeleteDto deleteDto,
            IEnumerable<TEntity> entities,
            Func<TEntity, int> idSelector,
            Func<TEntity, long> rowVersionSelector,
            Func<int, bool>? isAuthorizedId = null)
        {
            if (deleteDto?.DelIds == null || deleteDto.DelIds.Count == 0)
            {
                return false;
            }

            var expectedVersionGroups = deleteDto.DelIds
                .GroupBy(x => x.Id)
                .ToList();

            if (expectedVersionGroups.Any(g => g.Select(x => (long)x.RowVersion).Distinct().Count() > 1))
            {
                return true;
            }

            var expectedVersions = expectedVersionGroups
                .ToDictionary(g => g.Key, g => (long)g.First().RowVersion);

            var entityList = (entities ?? Enumerable.Empty<TEntity>()).ToList();
            isAuthorizedId ??= BuildDefaultAuthorizationPredicate(entityList, idSelector);

            if (isAuthorizedId != null && expectedVersions.Keys.Any(id => !isAuthorizedId(id)))
            {
                return true;
            }

            var actualVersions = entityList
                .GroupBy(idSelector)
                .ToDictionary(g => g.Key, g => rowVersionSelector(g.First()));

            if (expectedVersions.Count != actualVersions.Count)
            {
                return true;
            }

            foreach (var item in expectedVersions)
            {
                if (!actualVersions.TryGetValue(item.Key, out var actualVersion))
                {
                    return true;
                }

                if (actualVersion != item.Value)
                {
                    return true;
                }
            }

            return false;
        }

        private static Func<int, bool>? BuildDefaultAuthorizationPredicate<TEntity>(
            IEnumerable<TEntity> entities,
            Func<TEntity, int> idSelector)
        {
            var (currentUserNumber, isSuperAdmin) = GetCurrentUserContext();
            if (isSuperAdmin)
            {
                return _ => true;
            }

            if (string.IsNullOrWhiteSpace(currentUserNumber))
            {
                return null;
            }

            var ownerProperty = typeof(TEntity).GetProperty("DataInsUsr");
            if (ownerProperty == null || ownerProperty.PropertyType != typeof(string))
            {
                return null;
            }

            var ownerById = entities
                .GroupBy(idSelector)
                .ToDictionary(
                    g => g.Key,
                    g => ownerProperty.GetValue(g.First())?.ToString());

            return id =>
            {
                if (!ownerById.TryGetValue(id, out var owner))
                {
                    return false;
                }

                return string.IsNullOrWhiteSpace(owner)
                       || string.Equals(owner, currentUserNumber, StringComparison.OrdinalIgnoreCase);
            };
        }

        private static (string? UserNumber, bool IsSuperAdmin) GetCurrentUserContext()
        {
            ClaimsPrincipal? user = null;

            try
            {
                user = _httpContextAccessor?.HttpContext?.User;
            }
            catch
            {
                // ignored
            }

            user ??= Thread.CurrentPrincipal as ClaimsPrincipal;
            if (user == null)
            {
                return (null, false);
            }

            var userNumber = user.FindFirst(ClaimTypes.SerialNumber)?.Value
                             ?? user.FindFirst("serialnumber")?.Value
                             ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isSuperAdminClaim = user.FindFirst("is_super_admin")?.Value
                                    ?? user.FindFirst("isSuperAdmin")?.Value
                                    ?? user.FindFirst("issuperadmin")?.Value;

            return (userNumber, ParseBooleanLikeValue(isSuperAdminClaim));
        }

        private static bool ParseBooleanLikeValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
