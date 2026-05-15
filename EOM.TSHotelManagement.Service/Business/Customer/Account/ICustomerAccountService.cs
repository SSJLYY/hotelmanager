using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    public interface ICustomerAccountService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="readCustomerAccountInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadCustomerAccountOutputDto> Login(ReadCustomerAccountInputDto readCustomerAccountInputDto);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="readCustomerAccountInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadCustomerAccountOutputDto> Register(ReadCustomerAccountInputDto readCustomerAccountInputDto);

        /// <summary>
        /// 获取客户账号的 2FA 状态
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus(string customerSerialNumber);

        /// <summary>
        /// 生成客户账号的 2FA 绑定信息
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup(string customerSerialNumber);

        /// <summary>
        /// 启用客户账号 2FA
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor(string customerSerialNumber, TwoFactorCodeInputDto inputDto);

        /// <summary>
        /// 关闭客户账号 2FA
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        BaseResponse DisableTwoFactor(string customerSerialNumber, TwoFactorCodeInputDto inputDto);

        /// <summary>
        /// 重置客户账号恢复备用码
        /// </summary>
        /// <param name="customerSerialNumber">客户编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码或恢复备用码输入</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes(string customerSerialNumber, TwoFactorCodeInputDto inputDto);
    }
}
