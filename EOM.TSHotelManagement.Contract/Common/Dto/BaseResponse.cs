using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public class BaseResponse
    {
        public bool Success => Code == 0;

        /// <summary>
        /// 状态码，例如 0 表示成功
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// 返回消息，用于描述请求结果
        /// </summary>
        public string Message { get; set; } = LocalizationHelper.GetLocalizedString("Success", "成功");

        /// <summary>
        /// 
        /// </summary>
        public BaseResponse()
        {
            Code = 0;
            Message = LocalizationHelper.GetLocalizedString("Success", "成功");
        }

        /// <summary>
        /// 带状态码和消息的构造函数
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <param name="message">消息</param>
        public BaseResponse(int statusCode, string message)
        {
            Code = statusCode;
            Message = message;
        }
    }
}
