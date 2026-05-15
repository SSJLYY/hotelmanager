namespace EOM.TSHotelManagement.Contract
{
    public static class BusinessStatusCode
    {
        // 2xx Success
        /// <summary>
        /// 请求成功
        /// </summary>
        public const int Success = 0;

        /// <summary>
        /// 创建成功（常用于 POST 请求）
        /// </summary>
        public const int Created = 1201;

        /// <summary>
        /// 已接受（请求已接收但未处理完成）
        /// </summary>
        public const int Accepted = 1202;

        /// <summary>
        /// 无内容（响应体为空）
        /// </summary>
        public const int NoContent = 1204;

        // 3xx Redirection
        /// <summary>
        /// 永久重定向
        /// </summary>
        public const int MovedPermanently = 1301;

        /// <summary>
        /// 临时重定向
        /// </summary>
        public const int Found = 1302;

        /// <summary>
        /// 查看其他地址（常用于 POST 后重定向）
        /// </summary>
        public const int SeeOther = 1303;

        /// <summary>
        /// 资源未修改（缓存用）
        /// </summary>
        public const int NotModified = 1304;

        // 4xx Client Errors
        /// <summary>
        /// 错误请求（参数或格式错误）
        /// </summary>
        public const int BadRequest = 1400;

        /// <summary>
        /// 未授权（身份验证失败）
        /// </summary>
        public const int Unauthorized = 1401;

        /// <summary>
        /// 权限不足
        /// </summary>
        public const int PermissionDenied = 1402;

        /// <summary>
        /// 禁止访问（无权限）
        /// </summary>
        public const int Forbidden = 1403;

        /// <summary>
        /// 未找到资源
        /// </summary>
        public const int NotFound = 1404;

        /// <summary>
        /// 方法不允许（如 GET 接口用 POST 访问）
        /// </summary>
        public const int MethodNotAllowed = 1405;

        /// <summary>
        /// 请求超时
        /// </summary>
        public const int RequestTimeout = 1408;

        /// <summary>
        /// 资源冲突（如重复提交）
        /// </summary>
        public const int Conflict = 1409;

        // 16xx Idempotency
        /// <summary>
        /// 缺少幂等键
        /// </summary>
        public const int IdempotencyKeyMissing = 1601;

        /// <summary>
        /// 幂等键复用但请求体不一致
        /// </summary>
        public const int IdempotencyKeyPayloadConflict = 1602;

        /// <summary>
        /// 相同幂等键请求正在处理中
        /// </summary>
        public const int IdempotencyRequestInProgress = 1603;

        // 5xx Server Errors
        /// <summary>
        /// 服务器内部错误
        /// </summary>
        public const int InternalServerError = 1500;

        /// <summary>
        /// 网关错误（上游服务异常）
        /// </summary>
        public const int BadGateway = 1502;

        /// <summary>
        /// 服务不可用（维护或过载）
        /// </summary>
        public const int ServiceUnavailable = 1503;

        /// <summary>
        /// 网关超时（上游服务响应超时）
        /// </summary>
        public const int GatewayTimeout = 1504;
    }

}
