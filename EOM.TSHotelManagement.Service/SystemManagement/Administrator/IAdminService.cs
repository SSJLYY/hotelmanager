/*
 * MIT License
 *Copyright (c) 2021 易开元(Easy-Open-Meta)

 *Permission is hereby granted, free of charge, to any person obtaining a copy
 *of this software and associated documentation files (the "Software"), to deal
 *in the Software without restriction, including without limitation the rights
 *to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *copies of the Software, and to permit persons to whom the Software is
 *furnished to do so, subject to the following conditions:

 *The above copyright notice and this permission notice shall be included in all
 *copies or substantial portions of the Software.

 *THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *SOFTWARE.
 *
 */
using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 管理员数据访问接口
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// 后台系统登录
        /// </summary>
        /// <param name="readAdministratorInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadAdministratorOutputDto> Login(ReadAdministratorInputDto readAdministratorInputDto);

        /// <summary>
        /// 获取所有管理员列表
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadAdministratorOutputDto> GetAllAdminList(ReadAdministratorInputDto readAdministratorInputDto);

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="createAdministratorInputDto"></param>
        /// <returns></returns>
        BaseResponse AddAdmin(CreateAdministratorInputDto createAdministratorInputDto);

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="updateAdministratorInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdAdmin(UpdateAdministratorInputDto updateAdministratorInputDto);

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="deleteAdministratorInputDto"></param>
        /// <returns></returns>
        BaseResponse DelAdmin(DeleteAdministratorInputDto deleteAdministratorInputDto);

        /// <summary>
        /// 获取所有管理员类型
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadAdministratorTypeOutputDto> GetAllAdminTypes(ReadAdministratorTypeInputDto readAdministratorTypeInputDto);

        /// <summary>
        /// 添加管理员类型
        /// </summary>
        /// <param name="createAdministratorTypeInputDto"></param>
        /// <returns></returns>
        BaseResponse AddAdminType(CreateAdministratorTypeInputDto createAdministratorTypeInputDto);

        /// <summary>
        /// 更新管理员类型
        /// </summary>
        /// <param name="updateAdministratorTypeInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdAdminType(UpdateAdministratorTypeInputDto updateAdministratorTypeInputDto);

        /// <summary>
        /// 删除管理员类型
        /// </summary>
        /// <param name="deleteAdministratorTypeInputDto"></param>
        /// <returns></returns>
        BaseResponse DelAdminType(DeleteAdministratorTypeInputDto deleteAdministratorTypeInputDto);

        /// <summary>
        /// 为用户分配角色（全量覆盖）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseResponse AssignUserRoles(AssignUserRolesInputDto input);

        /// <summary>
        /// 读取指定用户已分配的角色编码集合
        /// </summary>
        /// <param name="userNumber">用户编码</param>
        /// <returns>角色编码集合（RoleNumber 列表）</returns>
        ListOutputDto<string> ReadUserRoles(string userNumber);

        /// <summary>
        /// 读取指定用户的“角色-权限”明细（来自 RolePermission 关联，并联到 Permission 得到权限码与名称）
        /// </summary>
        /// <param name="userNumber">用户编码</param>
        /// <returns>明细列表（包含 RoleNumber、PermissionNumber、PermissionName、MenuKey）</returns>
        ListOutputDto<UserRolePermissionOutputDto> ReadUserRolePermissions(string userNumber);

        /// <summary>
        /// 为指定用户分配“直接权限”（通过专属角色 R-USER-{UserNumber} 写入 RolePermission，全量覆盖）
        /// </summary>
        /// <param name="input">用户编号与权限编码集合</param>
        /// <returns></returns>
        BaseResponse AssignUserPermissions(AssignUserPermissionsInputDto input);

        /// <summary>
        /// 读取指定用户的“直接权限”（仅来自专属角色 R-USER-{UserNumber} 的权限编码列表）
        /// </summary>
        /// <param name="userNumber">用户编码</param>
        /// <returns>权限编码集合（PermissionNumber 列表）</returns>
        ListOutputDto<string> ReadUserDirectPermissions(string userNumber);

        /// <summary>
        /// 获取管理员账号的 2FA 状态
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus(string adminSerialNumber);

        /// <summary>
        /// 生成管理员账号的 2FA 绑定信息
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup(string adminSerialNumber);

        /// <summary>
        /// 启用管理员账号 2FA
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor(string adminSerialNumber, TwoFactorCodeInputDto inputDto);

        /// <summary>
        /// 关闭管理员账号 2FA
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        BaseResponse DisableTwoFactor(string adminSerialNumber, TwoFactorCodeInputDto inputDto);

        /// <summary>
        /// 重置管理员账号恢复备用码
        /// </summary>
        /// <param name="adminSerialNumber">管理员编号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码或恢复备用码输入</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes(string adminSerialNumber, TwoFactorCodeInputDto inputDto);
    }
}
