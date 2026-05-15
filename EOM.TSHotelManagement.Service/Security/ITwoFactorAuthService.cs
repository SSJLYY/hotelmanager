using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 统一 2FA 业务接口
    /// </summary>
    public interface ITwoFactorAuthService
    {
        /// <summary>
        /// 判断账号是否已启用 2FA
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="userPrimaryKey">账号主键ID</param>
        /// <returns>是否需要 2FA 校验</returns>
        bool RequiresTwoFactor(TwoFactorUserType userType, int userPrimaryKey);

        /// <summary>
        /// 校验登录场景的 2FA 验证码（支持 TOTP 或恢复备用码）
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="userPrimaryKey">账号主键ID</param>
        /// <param name="code">验证码或恢复备用码</param>
        /// <param name="usedRecoveryCode">是否使用了恢复备用码</param>
        /// <returns>是否校验通过</returns>
        bool VerifyLoginCode(TwoFactorUserType userType, int userPrimaryKey, string? code, out bool usedRecoveryCode);

        /// <summary>
        /// 获取当前账号的 2FA 状态
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <returns>2FA 状态</returns>
        SingleOutputDto<TwoFactorStatusOutputDto> GetStatus(TwoFactorUserType userType, string serialNumber);

        /// <summary>
        /// 生成 2FA 绑定信息（otpauth URI）
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <returns>绑定信息</returns>
        SingleOutputDto<TwoFactorSetupOutputDto> GenerateSetup(TwoFactorUserType userType, string serialNumber);

        /// <summary>
        /// 启用 2FA
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <param name="verificationCode">验证码</param>
        /// <returns>操作结果与首批恢复备用码</returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> Enable(TwoFactorUserType userType, string serialNumber, string verificationCode);

        /// <summary>
        /// 关闭 2FA
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <param name="verificationCode">验证码或恢复备用码</param>
        /// <returns>操作结果</returns>
        BaseResponse Disable(TwoFactorUserType userType, string serialNumber, string verificationCode);

        /// <summary>
        /// 重置恢复备用码（会使旧备用码全部失效）
        /// </summary>
        /// <param name="userType">账号类型</param>
        /// <param name="serialNumber">账号业务编号（JWT SerialNumber）</param>
        /// <param name="verificationCode">验证码或恢复备用码</param>
        /// <returns>新恢复备用码</returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateRecoveryCodes(TwoFactorUserType userType, string serialNumber, string verificationCode);
    }
}
