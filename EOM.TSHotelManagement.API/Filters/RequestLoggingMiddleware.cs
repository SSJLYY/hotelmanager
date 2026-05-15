using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _softwareVersion;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        IConfiguration config,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _softwareVersion = EOM.TSHotelManagement.Common.SoftwareVersionHelper.GetSoftwareVersion(config["SoftwareVersion"]);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = Stopwatch.GetTimestamp();
        Exception exception = null;
        var responseSize = 0L;
        string requestParameters = null;
        string responseContent = null;
        RequestLog log = null;

        context.Request.EnableBuffering();

        try
        {
            requestParameters = await GetRequestParametersAsync(context.Request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取请求参数失败");
            requestParameters = "Error reading request body";
        }

        log = new RequestLog
        {
            Path = context.Request.Path,
            Method = context.Request.Method,
            ClientIP = GetClientIp(context),
            Parameters = requestParameters,
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            UserName = context.User.Identity?.Name ?? "Anonymous",
            DataInsUsr = context.User.Identity?.Name ?? "Anonymous",
            DataInsDate = DateTime.Now,
            SoftwareVersion = _softwareVersion
        };

        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            log.ElapsedMilliseconds = GetElapsedMilliseconds(startTime, Stopwatch.GetTimestamp());

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            responseContent = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseSize = Encoding.UTF8.GetByteCount(responseContent);

            log.StatusCode = context.Response.StatusCode;
            log.ResponseSize = responseSize;

            if (context.Response.StatusCode < 200 || context.Response.StatusCode >= 300)
            {
                log.Exception = responseContent.Length > 2000 ?
                    responseContent.Substring(0, 2000) : responseContent;
            }

            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            exception = ex;
            log.Exception = ex.ToString();
            log.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.StatusCode = log.StatusCode;

            context.Response.Body = originalBodyStream;

            await HandleExceptionAsync(context, ex);
        }
        finally
        {
            context.Response.Body = originalBodyStream;

            log.ActionName = context.GetEndpoint()?.DisplayName;
            log.DataChgUsr = context.User.Identity?.Name ?? "Anonymous";
            log.DataChgDate = DateTime.Now;

            try
            {
                var repository = context.RequestServices.GetRequiredService<GenericRepository<RequestLog>>();
                await repository.InsertAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "日志记录失败");
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted || !context.Response.Body.CanWrite)
        {
            _logger.LogError("无法写入错误响应：响应已开始或流不可写");
            return;
        }

        try
        {
            context.Response.ContentType = "application/json";
            var errorResponse = new { error = exception.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理异常时发生错误");
        }
    }

    private static long GetElapsedMilliseconds(long start, long stop)
    {
        return (long)((stop - start) * 1000 / (double)Stopwatch.Frequency);
    }

    private async Task<string> GetRequestParametersAsync(HttpRequest request)
    {
        if (request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            return request.QueryString.HasValue ? request.QueryString.Value : null;
        }

        if (request.ContentLength == null || request.ContentLength == 0)
            return null;

        try
        {
            request.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var parameters = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            return parameters.Length > 8000 ? parameters.Substring(0, 8000) : parameters;
        }
        catch
        {
            return "Unable to read request body";
        }
    }

    private string GetClientIp(HttpContext context)
    {
        var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xForwardedFor))
        {
            var ips = xForwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var ip in ips)
            {
                var cleanIp = ip.Trim();
                if (!IsLocalIp(cleanIp))
                {
                    return cleanIp;
                }
            }
            return ips.FirstOrDefault()?.Trim() ?? string.Empty;
        }

        var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(xRealIp) && !IsLocalIp(xRealIp))
        {
            return xRealIp.Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private bool IsLocalIp(string ip)
    {
        return ip == "::1" ||
               ip == "127.0.0.1" ||
               ip.StartsWith("192.168.") ||
               ip.StartsWith("10.") ||
               ip.StartsWith("172.16.") ||
               ip.StartsWith("172.17.") ||
               ip.StartsWith("172.18.") ||
               ip.StartsWith("172.19.") ||
               ip.StartsWith("172.20.") ||
               ip.StartsWith("172.21.") ||
               ip.StartsWith("172.22.") ||
               ip.StartsWith("172.23.") ||
               ip.StartsWith("172.24.") ||
               ip.StartsWith("172.25.") ||
               ip.StartsWith("172.26.") ||
               ip.StartsWith("172.27.") ||
               ip.StartsWith("172.28.") ||
               ip.StartsWith("172.29.") ||
               ip.StartsWith("172.30.") ||
               ip.StartsWith("172.31.");
    }
}
