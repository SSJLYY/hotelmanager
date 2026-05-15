namespace EOM.TSHotelManagement.Contract
{
    public class ReadOperationLogOutputDto : BaseOutputDto
    {
        public string OperationId { get; set; }
        public DateTime OperationTime { get; set; }
        public string LogContent { get; set; }
        public string OperationAccount { get; set; }
        public int LogLevel { get; set; }
        public string SoftwareVersion { get; set; }
        public string LoginIpAddress { get; set; }
        public string LogLevelName { get; set; }
        /// <summary>
        /// 请求路径 (Request Path)
        /// </summary>
        public string RequestPath { get; set; }
        /// <summary>
        /// 响应时间 (Elapsed Time)
        /// </summary>
        public long ElapsedTime { get; set; }
        /// <summary>
        /// 请求方法 (Http Method)
        /// </summary>
        public string HttpMethod { get; set; }
        /// <summary>
        /// 状态码 (Status Code)
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 异常消息 (Exception Message)
        /// </summary>
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// 异常堆栈 (Exception Stack Trace)
        /// </summary>
        public string ExceptionStackTrace { get; set; }
    }
}
