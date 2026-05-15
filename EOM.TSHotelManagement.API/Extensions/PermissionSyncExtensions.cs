using EOM.TSHotelManagement.Domain;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EOM.TSHotelManagement.WebApi
{
    /// <summary>
    /// 启动时扫描所有带有 RequirePermissionAttribute 的控制器动作，
    /// 将缺失的权限码自动初始化到数据库，并默认关联到“超级管理员”角色。
    /// </summary>
    public static class PermissionSyncExtensions
    {
        private const string AdminRoleNumber = "R-ADMIN";

        /// <summary>
        /// 扫描并同步权限
        /// </summary>
        public static void SyncPermissionsFromAttributes(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("PermissionSync");
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
            var actionProvider = scope.ServiceProvider.GetRequiredService<IActionDescriptorCollectionProvider>();

            try
            {
                var descriptors = actionProvider.ActionDescriptors.Items.OfType<ControllerActionDescriptor>().ToList();

                var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var cad in descriptors)
                {
                    // 方法级
                    foreach (var attr in cad.MethodInfo.GetCustomAttributes(inherit: true))
                    {
                        if (attr is RequirePermissionAttribute rpa && !string.IsNullOrWhiteSpace(rpa.PermissionCode))
                        {
                            codes.Add(rpa.PermissionCode);
                        }
                    }

                    // 控制器级（以防未来使用）
                    foreach (var attr in cad.ControllerTypeInfo.GetCustomAttributes(inherit: true))
                    {
                        if (attr is RequirePermissionAttribute rpa && !string.IsNullOrWhiteSpace(rpa.PermissionCode))
                        {
                            codes.Add(rpa.PermissionCode);
                        }
                    }
                }

                if (codes.Count == 0)
                {
                    logger.LogInformation("PermissionSync: 未发现任何 RequirePermissionAttribute 标注的接口，跳过同步。");
                    return;
                }

                // 查询已有权限
                var existing = db.Queryable<Permission>()
                    .Select(p => p.PermissionNumber)
                    .ToList();

                var toInsert = codes
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Except(existing, StringComparer.OrdinalIgnoreCase)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (toInsert.Count > 0)
                {
                    var now = DateTime.Now;
                    var newPerms = toInsert.Select(code => new Permission
                    {
                        PermissionNumber = code,
                        PermissionName = code, // 默认使用编码作为名称
                        Module = GuessModule(code),
                        Description = "Auto-synced from [RequirePermission] on API action",
                        MenuKey = null,
                        ParentNumber = null,
                        DataInsUsr = "System",
                        DataInsDate = now
                    }).ToList();

                    db.Insertable(newPerms).ExecuteCommand();
                    logger.LogInformation("PermissionSync: 已插入 {Count} 条新权限。", newPerms.Count);
                }
                else
                {
                    logger.LogInformation("PermissionSync: 所有标注的权限均已存在，无需插入。");
                }

                // 默认将所有扫描到的权限与超级管理员角色关联
                // 如角色不存在，DatabaseInitializer 已负责创建；此处仅做映射补齐
                var existingRolePerms = db.Queryable<RolePermission>()
                    .Where(rp => rp.RoleNumber == AdminRoleNumber
                              && rp.PermissionNumber != null
                              && rp.PermissionNumber != "")
                    .Select(rp => rp.PermissionNumber)
                    .ToList();

                var toGrant = codes
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Except(existingRolePerms, StringComparer.OrdinalIgnoreCase)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                if (toGrant.Count > 0)
                {
                    var now = DateTime.Now;
                    var inserts = toGrant.Select(p => new RolePermission
                    {
                        RoleNumber = AdminRoleNumber,
                        PermissionNumber = p,
                        DataInsUsr = "System",
                        DataInsDate = now
                    }).ToList();

                    db.Insertable(inserts).ExecuteCommand();
                    logger.LogInformation("PermissionSync: 已为超级管理员角色授予 {Count} 条权限。", inserts.Count);
                }
                else
                {
                    logger.LogInformation("PermissionSync: 超级管理员已具备所有扫描到的权限。");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PermissionSync: 同步权限时发生异常。");
            }
        }

        private static string GuessModule(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return "api";

            var colon = code.IndexOf(':');
            if (colon > 0)
            {
                return code.Substring(0, colon);
            }

            // 兼容 "xxx.yyy" 风格
            var dot = code.IndexOf('.');
            if (dot > 0)
            {
                return code.Substring(0, dot);
            }

            return "api";
        }
    }
}
