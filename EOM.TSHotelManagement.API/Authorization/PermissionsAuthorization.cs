using EOM.TSHotelManagement.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SqlSugar;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.WebApi.Authorization
{
    /// <summary>
    /// 权限策略常量
    /// </summary>
    public static class PermissionPolicyConstants
    {
        public const string Prefix = "PERM:";
    }

    /// <summary>
    /// 以权限码为单位的授权特性，使用示例： [RequirePermission("system:role:list")]
    /// </summary>
    public class RequirePermissionAttribute : AuthorizeAttribute
    {
        public string PermissionCode { get; }

        public RequirePermissionAttribute(string permissionCode)
        {
            PermissionCode = permissionCode ?? throw new ArgumentNullException(nameof(permissionCode));
            Policy = PermissionPolicyConstants.Prefix + permissionCode;
        }
    }

    /// <summary>
    /// 权限需求对象
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Code { get; }

        public PermissionRequirement(string code)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }
    }

    /// <summary>
    /// 自定义策略提供者：当遇到以 "PERM:" 开头的策略名时，动态创建权限策略
    /// </summary>
    public class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName != null && policyName.StartsWith(PermissionPolicyConstants.Prefix, StringComparison.OrdinalIgnoreCase))
            {
                var code = policyName.Substring(PermissionPolicyConstants.Prefix.Length);
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new PermissionRequirement(code))
                    .Build();
                return Task.FromResult(policy);
            }

            return base.GetPolicyAsync(policyName);
        }
    }

    /// <summary>
    /// 权限授权处理器：判断当前用户是否具备所需权限
    /// 规则：
    /// 1) 如果用户是超级管理员(IsSuperAdmin == 1) -> 直接放行
    /// 2) 否则：用户-角色(UserRole) -> 角色-权限(RolePermission) 中存在目标权限码且未被删除(IsDelete != 1) -> 放行
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ISqlSugarClient _db;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<PermissionAuthorizationHandler> _logger;

        public PermissionAuthorizationHandler(
            ISqlSugarClient db,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PermissionAuthorizationHandler> logger)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var user = httpContext?.User;

                if (user?.Identity?.IsAuthenticated != true)
                {
                    // Unauthenticated requests should be handled by authentication challenge,
                    // not by permission checks.
                    return;
                }

                var userNumber = user?.FindFirst(ClaimTypes.SerialNumber)?.Value
                                 ?? user?.FindFirst("serialnumber")?.Value;

                if (string.IsNullOrWhiteSpace(userNumber))
                {
                    userNumber = GetUserNumberFromAuthorizationHeader(httpContext);
                }

                if (string.IsNullOrWhiteSpace(userNumber))
                {
                    _logger.LogWarning("权限校验失败：无法获取用户编号(SerialNumber)");
                    return;
                }

                // 超级管理员直接通过
                var isSuperAdmin = await _db.Queryable<Administrator>()
                    .Where(a => a.Number == userNumber && a.IsDelete != 1)
                    .Select(a => a.IsSuperAdmin == 1)
                    .FirstAsync();

                if (isSuperAdmin)
                {
                    context.Succeed(requirement);
                    return;
                }

                // 获取用户绑定的有效角色
                var roleNumbers = await _db.Queryable<UserRole>()
                    .Where(ur => ur.UserNumber == userNumber)
                    .Select(ur => ur.RoleNumber)
                    .ToListAsync();

                if (roleNumbers == null || roleNumbers.Count == 0)
                {
                    _logger.LogInformation("用户 {UserNumber} 未绑定任何角色", userNumber);
                    return;
                }

                // 判断角色是否拥有该权限
                var hasPermission = await _db.Queryable<RolePermission>()
                    .Where(rp => roleNumbers.Contains(rp.RoleNumber)
                                && rp.PermissionNumber == requirement.Code)
                    .AnyAsync();

                if (hasPermission)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    _logger.LogInformation("用户 {UserNumber} 无权限码 {Perm}", userNumber, requirement.Code);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "权限校验异常");
            }
        }

        /// <summary>
        /// 从Authorization头解析用户编号（解决Swagger文档请求问题）
        /// </summary>
        private string GetUserNumberFromAuthorizationHeader(HttpContext httpContext)
        {
            if (httpContext?.Request?.Headers == null) return null;

            if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString();
                if (string.IsNullOrWhiteSpace(token)) return null;

                // 移除"Bearer "前缀
                if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = token.Substring(7).Trim();
                }

                // 解析JWT token获取用户编号
                try
                {
                    var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadJwtToken(token);

                    return jwtToken.Claims
                        .FirstOrDefault(c =>
                            c.Type == ClaimTypes.SerialNumber ||
                            c.Type == "serialnumber")?.Value;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "解析JWT token失败");
                    return null;
                }
            }
            return null;
        }
    }
}
