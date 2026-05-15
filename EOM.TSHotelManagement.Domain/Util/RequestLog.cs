using SqlSugar;
using System;

namespace EOM.TSHotelManagement.Domain
{
    [SugarTable("request_log", "请求日志表 (Request Log)")]
    public class RequestLog : AuditEntity
    {
        /// <summary>
        /// 编号 (ID)
        /// </summary>
        [SugarColumn(ColumnName = "id", IsIdentity = true, IsPrimaryKey = true, IsNullable = false, ColumnDescription = "编号 (ID)")]
        public int Id { get; set; }

        /// <summary>
        /// 请求路径 (URL Path)
        /// </summary>
        [SugarColumn(ColumnName = "path", IsNullable = false, Length = 500, ColumnDescription = "请求路径 (URL Path)")]
        public string Path { get; set; }

        /// <summary>
        /// HTTP方法 (GET/POST等)
        /// </summary>
        [SugarColumn(ColumnName = "method", Length = 10, IsNullable = false, ColumnDescription = "HTTP方法 (Method)")]
        public string Method { get; set; }

        /// <summary>
        /// 请求IP地址
        /// </summary>
        [SugarColumn(ColumnName = "client_ip", Length = 50, IsNullable = false, ColumnDescription = "客户端IP (Client IP)")]
        public string ClientIP { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        [SugarColumn(ColumnName = "parameters", ColumnDataType = "text", IsNullable = true, ColumnDescription = "请求参数 (Parameters)")]
        public string Parameters { get; set; }

        /// <summary>
        /// HTTP状态码
        /// </summary>
        [SugarColumn(ColumnName = "status_code", ColumnDescription = "HTTP状态码 (Status Code)")]
        public int StatusCode { get; set; }

        /// <summary>
        /// 请求时长(毫秒)
        /// </summary>
        [SugarColumn(ColumnName = "elapsed_ms", ColumnDescription = "请求耗时(毫秒) (Elapsed Milliseconds)")]
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 用户代理 (浏览器信息)
        /// </summary>
        [SugarColumn(ColumnName = "user_agent", Length = 500, IsNullable = true, ColumnDescription = "用户代理 (User Agent)")]
        public string UserAgent { get; set; }

        /// <summary>
        /// 用户名 (未登录则为空)
        /// </summary>
        [SugarColumn(ColumnName = "user_name", Length = 100, IsNullable = true, ColumnDescription = "用户名 (User Name)")]
        public string UserName { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        [SugarColumn(ColumnName = "exception", ColumnDataType = "text", IsNullable = true, ColumnDescription = "异常信息 (Exception)")]
        public string Exception { get; set; }

        /// <summary>
        /// 请求时间 (自动设置为当前时间)
        /// </summary>
        [SugarColumn(ColumnName = "request_time", IsNullable = false, ColumnDescription = "请求时间 (Request Time)")]
        public DateTime RequestTime { get; } = DateTime.Now;

        /// <summary>
        /// 响应大小(字节)
        /// </summary>
        [SugarColumn(ColumnName = "response_size", ColumnDescription = "响应大小(字节) (Response Size)")]
        public long ResponseSize { get; set; }

        /// <summary>
        /// API操作描述
        /// </summary>
        [SugarColumn(ColumnName = "action_name", Length = 200, IsNullable = true, ColumnDescription = "API操作 (API Action)")]
        public string ActionName { get; set; }

        /// <summary>
        /// 软件版本
        /// </summary>
        [SugarColumn(ColumnName = "software_version", Length = 20, IsNullable = true, ColumnDescription = "软件版本")]
        public string SoftwareVersion { get; set; }

    }
}
