namespace EOM.TSHotelManagement.WebApi.Authorization
{
    using EOM.TSHotelManagement.Common;
    using EOM.TSHotelManagement.Contract;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Policy;
    using Microsoft.AspNetCore.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private const string AuthFailureReasonItemKey = JwtAuthConstants.AuthFailureReasonItemKey;
        private const string AuthFailureReasonTokenRevoked = JwtAuthConstants.AuthFailureReasonTokenRevoked;
        private const string AuthFailureReasonTokenExpired = JwtAuthConstants.AuthFailureReasonTokenExpired;

        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (authorizeResult.Challenged)
            {
                var response = new BaseResponse(
                    BusinessStatusCode.Unauthorized,
                    ResolveUnauthorizedMessage(context));

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json; charset=utf-8";

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    DictionaryKeyPolicy = null
                });

                await context.Response.WriteAsync(json);
                return;
            }

            if (authorizeResult.Forbidden)
            {
                var response = new BaseResponse(
                    BusinessStatusCode.PermissionDenied,
                    LocalizationHelper.GetLocalizedString("PermissionDenied", "该账户缺少权限，请联系管理员添加"));

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json; charset=utf-8";

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    DictionaryKeyPolicy = null
                });

                await context.Response.WriteAsync(json);
                return;
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }

        private static string ResolveUnauthorizedMessage(HttpContext context)
        {
            if (context.Items.TryGetValue(AuthFailureReasonItemKey, out var reasonObj))
            {
                var reason = reasonObj?.ToString();
                if (string.Equals(reason, AuthFailureReasonTokenRevoked, System.StringComparison.OrdinalIgnoreCase))
                {
                    return LocalizationHelper.GetLocalizedString(
                        "Token has been revoked. Please log in again.",
                        "该Token已失效，请重新登录");
                }

                if (string.Equals(reason, AuthFailureReasonTokenExpired, System.StringComparison.OrdinalIgnoreCase))
                {
                    return LocalizationHelper.GetLocalizedString(
                        "Token has expired. Please log in again.",
                        "登录已过期，请重新登录");
                }
            }

            return LocalizationHelper.GetLocalizedString(
                "Unauthorized. Please log in again.",
                "未授权或登录已失效，请重新登录");
        }
    }
}
