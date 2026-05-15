using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using EOM.TSHotelManagement.WebApi.Authorization;
using jvncorelib.CodeLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.WebApi.Filters
{
    public class BusinessOperationAuditFilter : IAsyncActionFilter
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();
        private static readonly Regex LogControlCharsRegex = new(@"[\p{Cc}\p{Cf}]+", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly HashSet<string> ExcludedControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "Login",
            "Utility",
            "CustomerAccount",
            "News"
        };

        private static readonly string[] ReadOnlyActionPrefixes =
        {
            "Select",
            "Read",
            "Get",
            "Build"
        };

        private static readonly string[] SensitivePropertyKeywords =
        {
            "password",
            "token",
            "secret",
            "recoverycode",
            "verificationcode",
            "otp",
            "creditcard",
            "ssn",
            "bankaccount",
            "phonenumber"
        };

        private readonly GenericRepository<OperationLog> operationLogRepository;
        private readonly ILogger<BusinessOperationAuditFilter> logger;

        public BusinessOperationAuditFilter(
            GenericRepository<OperationLog> operationLogRepository,
            ILogger<BusinessOperationAuditFilter> logger)
        {
            this.operationLogRepository = operationLogRepository;
            this.logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldAudit(context))
            {
                await next();
                return;
            }

            var executedContext = await next();
            await TryWriteAuditLogAsync(context, executedContext);
        }

        private static bool ShouldAudit(ActionExecutingContext context)
        {
            if (context.ActionDescriptor is not ControllerActionDescriptor actionDescriptor)
            {
                return false;
            }

            if (!IsBusinessController(actionDescriptor))
            {
                return false;
            }

            if (ExcludedControllers.Contains(actionDescriptor.ControllerName))
            {
                return false;
            }

            if (HttpMethods.IsGet(context.HttpContext.Request.Method)
                || HttpMethods.IsHead(context.HttpContext.Request.Method)
                || HttpMethods.IsOptions(context.HttpContext.Request.Method))
            {
                return false;
            }

            return !ReadOnlyActionPrefixes.Any(prefix =>
                actionDescriptor.ActionName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsBusinessController(ControllerActionDescriptor actionDescriptor)
        {
            if (actionDescriptor.ControllerTypeInfo.IsDefined(typeof(BusinessOperationAuditAttribute), inherit: true))
            {
                return true;
            }

            var controllerNamespace = actionDescriptor.ControllerTypeInfo.Namespace ?? string.Empty;
            return controllerNamespace.Contains(".Controllers.Business.", StringComparison.OrdinalIgnoreCase);
        }

        private async Task TryWriteAuditLogAsync(ActionExecutingContext context, ActionExecutedContext executedContext)
        {
            try
            {
                var httpContext = context?.HttpContext;
                if (httpContext == null)
                {
                    logger.LogWarning("HttpContext is null, skipping audit log.");
                    return;
                }

                var request = httpContext.Request;
                var path = request.Path.HasValue ? request.Path.Value! : string.Empty;
                var isChinese = IsChineseLanguage(httpContext);
                var operationAccount = ClaimsPrincipalExtensions.GetUserNumber(httpContext.User);
                if (string.IsNullOrWhiteSpace(operationAccount))
                {
                    operationAccount = Localize(isChinese, "Anonymous", "匿名用户");
                }

                var (isSuccess, responseCode, responseMessage) = ResolveExecutionResult(executedContext);
                var argumentSummary = BuildArgumentSummary(context.ActionArguments, isChinese);
                var statusText = Localize(isChinese, isSuccess ? "Success" : "Failed", isSuccess ? "成功" : "失败");
                var logContent = BuildLogContent(
                    isChinese,
                    statusText,
                    request.Method,
                    path,
                    operationAccount,
                    argumentSummary,
                    responseCode,
                    responseMessage);

                var operationLog = new OperationLog
                {
                    OperationId = new UniqueCode().GetNewId("OP-"),
                    OperationTime = DateTime.Now,
                    LogContent = logContent,
                    LoginIpAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                    OperationAccount = operationAccount,
                    LogLevel = isSuccess ? (int)Common.LogLevel.Normal : (int)Common.LogLevel.Warning,
                    SoftwareVersion = SoftwareVersionHelper.GetSoftwareVersion(),
                    DataInsUsr = operationAccount,
                    DataInsDate = DateTime.Now
                };

                operationLogRepository.Insert(operationLog);
            }
            catch (Exception ex)
            {
                var path = context?.HttpContext?.Request?.Path.Value ?? string.Empty;
                logger.LogWarning(ex, "Failed to write business operation audit log for {Path}", path);
            }

            await Task.CompletedTask;
        }

        private static string BuildLogContent(
            bool isChinese,
            string statusText,
            string method,
            string path,
            string operationAccount,
            string argumentSummary,
            int responseCode,
            string responseMessage)
        {
            var safeStatusText = SanitizeForLog(statusText, 32);
            var safeMethod = SanitizeForLog(method, 16);
            var safePath = SanitizeForLog(path, 240);
            var safeOperationAccount = SanitizeForLog(operationAccount, 80);
            var safeArgumentSummary = SanitizeForLog(argumentSummary, 900);
            var localizedResponseMessage = string.IsNullOrWhiteSpace(responseMessage)
                ? Localize(isChinese, "None", "无")
                : SanitizeForLog(responseMessage, 300);

            var content = Localize(
                isChinese,
                $"{safeStatusText} {safeMethod} {safePath} | User={safeOperationAccount} | Args={safeArgumentSummary} | ResponseCode={responseCode} | Message={localizedResponseMessage}",
                $"{safeStatusText} {safeMethod} {safePath} | 操作人={safeOperationAccount} | 参数={safeArgumentSummary} | 响应码={responseCode} | 响应信息={localizedResponseMessage}");

            return TrimLogContent(content, isChinese);
        }

        private static string SanitizeForLog(string value, int maxLength = 300)
        {
            var sanitized = LogControlCharsRegex.Replace(value ?? string.Empty, " ").Trim();
            return sanitized.Length <= maxLength ? sanitized : sanitized[..(maxLength - 3)] + "...";
        }

        private static (bool IsSuccess, int ResponseCode, string ResponseMessage) ResolveExecutionResult(ActionExecutedContext executedContext)
        {
            if (executedContext.Exception != null && !executedContext.ExceptionHandled)
            {
                return (false, BusinessStatusCode.InternalServerError, executedContext.Exception.Message);
            }

            var baseResponse = ExtractBaseResponse(executedContext.Result);
            if (baseResponse != null)
            {
                return (baseResponse.Success, baseResponse.Code, baseResponse.Message ?? string.Empty);
            }

            if (executedContext.Result is StatusCodeResult statusCodeResult)
            {
                return (statusCodeResult.StatusCode < 400, statusCodeResult.StatusCode, string.Empty);
            }

            return (true, 0, string.Empty);
        }

        private static BaseResponse ExtractBaseResponse(IActionResult result)
        {
            return result switch
            {
                ObjectResult objectResult when objectResult.Value is BaseResponse baseResponse => baseResponse,
                JsonResult jsonResult when jsonResult.Value is BaseResponse baseResponse => baseResponse,
                _ => null
            };
        }

        private static string BuildArgumentSummary(IDictionary<string, object> arguments, bool isChinese)
        {
            if (arguments == null || arguments.Count == 0)
            {
                return Localize(isChinese, "None", "无");
            }

            var items = arguments
                .Where(a => a.Value != null)
                .Select(a => $"{SanitizeForLog(a.Key, 60)}={SummarizeValue(a.Key, a.Value, isChinese)}")
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToList();

            return items.Count == 0 ? Localize(isChinese, "None", "无") : string.Join("; ", items);
        }

        private static string SummarizeValue(string name, object value, bool isChinese)
        {
            if (value == null)
            {
                return Localize(isChinese, "null", "空");
            }

            if (IsSensitive(name))
            {
                return "***";
            }

            if (value is IFormFile file)
            {
                var safeFileName = SanitizeForLog(file.FileName, 120);
                return Localize(
                    isChinese,
                    $"File({safeFileName}, {file.Length} bytes)",
                    $"文件({safeFileName}, {file.Length} 字节)");
            }

            if (IsSimpleValue(value.GetType()))
            {
                return FormatSimpleValue(value);
            }

            if (value is IEnumerable enumerable && value is not string)
            {
                return SummarizeEnumerable(enumerable, isChinese);
            }

            return SummarizeComplexObject(value, isChinese);
        }

        private static string SummarizeEnumerable(IEnumerable enumerable, bool isChinese)
        {
            var count = 0;
            var previews = new List<string>();

            foreach (var item in enumerable)
            {
                count++;
                if (previews.Count >= 3)
                {
                    continue;
                }

                previews.Add(item == null ? Localize(isChinese, "null", "空") : FormatSimpleValue(item));
            }

            return previews.Count == 0
                ? "[]"
                : Localize(
                    isChinese,
                    $"[{string.Join(", ", previews)}{(count > previews.Count ? ", ..." : string.Empty)}] (Count={count})",
                    $"[{string.Join(", ", previews)}{(count > previews.Count ? ", ..." : string.Empty)}]（数量={count}）");
        }

        private static string SummarizeComplexObject(object value, bool isChinese)
        {
            var type = value.GetType();
            var properties = GetCachedProperties(type)
                .Take(8)
                .Select(p =>
                {
                    object propertyValue;
                    try
                    {
                        propertyValue = p.GetValue(value);
                    }
                    catch
                    {
                        return null;
                    }

                    if (propertyValue == null)
                    {
                        return null;
                    }

                    var safePropertyName = SanitizeForLog(p.Name, 60);

                    if (IsSensitive(p.Name))
                    {
                        return $"{safePropertyName}=***";
                    }

                    if (propertyValue is IFormFile file)
                    {
                        var safeFileName = SanitizeForLog(file.FileName, 120);
                        return Localize(
                            isChinese,
                            $"{safePropertyName}=File({safeFileName}, {file.Length} bytes)",
                            $"{safePropertyName}=文件({safeFileName}, {file.Length} 字节)");
                    }

                    if (IsSimpleValue(propertyValue.GetType()))
                    {
                        return $"{safePropertyName}={FormatSimpleValue(propertyValue)}";
                    }

                    if (propertyValue is IEnumerable enumerable && propertyValue is not string)
                    {
                        return $"{safePropertyName}={SummarizeEnumerable(enumerable, isChinese)}";
                    }

                    return null;
                })
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            var safeTypeName = SanitizeForLog(type.Name, 80);
            return properties.Count == 0
                ? safeTypeName
                : $"{safeTypeName}({string.Join(", ", properties)})";
        }

        private static PropertyInfo[] GetCachedProperties(Type type)
        {
            return PropertyCache.GetOrAdd(type, static currentType => currentType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                .ToArray());
        }

        private static bool IsSimpleValue(Type type)
        {
            var actualType = Nullable.GetUnderlyingType(type) ?? type;
            return actualType.IsPrimitive
                || actualType.IsEnum
                || actualType == typeof(string)
                || actualType == typeof(decimal)
                || actualType == typeof(Guid)
                || actualType == typeof(DateTime)
                || actualType == typeof(DateTimeOffset)
                || actualType == typeof(TimeSpan);
        }

        private static bool IsSensitive(string name)
        {
            var normalized = name?.Replace("_", string.Empty, StringComparison.OrdinalIgnoreCase) ?? string.Empty;
            return SensitivePropertyKeywords.Any(keyword =>
                normalized.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }

        private static string FormatSimpleValue(object value)
        {
            var formatted = value switch
            {
                DateTime dateTime => dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture),
                string text => text,
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? value.ToString() ?? string.Empty
            };

            return SanitizeForLog(formatted, 120);
        }

        private static string TrimLogContent(string content, bool isChinese)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Localize(isChinese, "Operation audit log", "业务操作审计日志");
            }

            return SanitizeForLog(content, 1900);
        }

        private static bool IsChineseLanguage(HttpContext httpContext)
        {
            var acceptLanguage = httpContext.Request.Headers.AcceptLanguage.ToString();
            if (!string.IsNullOrWhiteSpace(acceptLanguage))
            {
                var firstLanguage = acceptLanguage
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(item => item.Split(';', 2, StringSplitOptions.TrimEntries)[0])
                    .FirstOrDefault();

                if (!string.IsNullOrWhiteSpace(firstLanguage))
                {
                    return firstLanguage.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
                }
            }

            var cultureName = CultureInfo.CurrentUICulture?.Name;
            if (!string.IsNullOrWhiteSpace(cultureName))
            {
                return cultureName.StartsWith("zh", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private static string Localize(bool isChinese, string englishText, string chineseText)
        {
            return isChinese ? chineseText : englishText;
        }
    }
}
