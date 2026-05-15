using EOM.TSHotelManagement.Common;

namespace EOM.TSHotelManagement.Contract
{
    public static class BaseResponseFactory
    {
        public static BaseResponse ConcurrencyConflict()
        {
            return new BaseResponse(
                BusinessStatusCode.Conflict,
                LocalizationHelper.GetLocalizedString(
                    "Data has been modified by another user. Please refresh and retry.",
                    "数据已被其他用户修改，请刷新后重试。"));
        }
    }
}
