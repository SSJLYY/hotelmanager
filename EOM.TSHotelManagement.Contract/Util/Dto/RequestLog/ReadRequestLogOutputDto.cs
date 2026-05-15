namespace EOM.TSHotelManagement.Contract
{
    public class ReadRequestLogOutputDto : BaseOutputDto
    {

        /// <summary>
        /// 请求路径 (URL Path)
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// HTTP方法 (GET/POST等)
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求IP地址
        /// </summary>
        public string ClientIP { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// HTTP状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 请求时长(毫秒)
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 用户代理 (浏览器信息)
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 用户名 (未登录则为空)
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// 请求时间 (自动设置为当前时间)
        /// </summary>
        public DateTime RequestTime { get; } = DateTime.Now;

        /// <summary>
        /// 响应大小(字节)
        /// </summary>
        public long ResponseSize { get; set; }

        /// <summary>
        /// API操作描述
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 软件版本
        /// </summary>
        public string SoftwareVersion { get; set; }
    }
}