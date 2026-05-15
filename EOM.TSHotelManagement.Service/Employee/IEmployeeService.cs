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
    /// 员工信息接口
    /// </summary>
    public interface IEmployeeService
    {
        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdateEmployee(UpdateEmployeeInputDto updateEmployeeInputDto);

        /// <summary>
        /// 员工账号禁/启用
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        BaseResponse ManagerEmployeeAccount(UpdateEmployeeInputDto updateEmployeeInputDto);

        /// <summary>
        /// 添加员工信息
        /// </summary>
        /// <param name="createEmployeeInputDto"></param>
        /// <returns></returns>
        BaseResponse AddEmployee(CreateEmployeeInputDto createEmployeeInputDto);

        /// <summary>
        /// 获取所有工作人员信息
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadEmployeeOutputDto> SelectEmployeeAll(ReadEmployeeInputDto readEmployeeInputDto);

        /// <summary>
        /// 根据登录名称查询员工信息
        /// </summary>
        /// <param name="readEmployeeInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadEmployeeOutputDto> SelectEmployeeInfoByEmployeeId(ReadEmployeeInputDto readEmployeeInputDto);

        /// <summary>
        /// 员工端登录
        /// </summary>
        /// <param name="employeeLoginDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadEmployeeOutputDto> EmployeeLogin(EmployeeLoginDto employeeLoginDto);

        /// <summary>
        /// 修改员工账号密码
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdateEmployeeAccountPassword(UpdateEmployeeInputDto updateEmployeeInputDto);

        /// <summary>
        /// 重置员工账号密码
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        BaseResponse ResetEmployeeAccountPassword(UpdateEmployeeInputDto updateEmployeeInputDto);

        /// <summary>
        /// 获取员工账号的 2FA 状态
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus(string employeeSerialNumber);

        /// <summary>
        /// 生成员工账号的 2FA 绑定信息
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup(string employeeSerialNumber);

        /// <summary>
        /// 启用员工账号 2FA
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor(string employeeSerialNumber, TwoFactorCodeInputDto inputDto);

        /// <summary>
        /// 关闭员工账号 2FA
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        BaseResponse DisableTwoFactor(string employeeSerialNumber, TwoFactorCodeInputDto inputDto);

        /// <summary>
        /// 重置员工账号恢复备用码
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码或恢复备用码输入</param>
        /// <returns></returns>
        SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes(string employeeSerialNumber, TwoFactorCodeInputDto inputDto);
    }
}
