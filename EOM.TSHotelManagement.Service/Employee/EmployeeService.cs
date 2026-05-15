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
using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Common.Helper;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.Extensions.Logging;
using SqlSugar;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;

namespace EOM.TSHotelManagement.Service.Employee
{
    /// <summary>
    /// 员工信息接口实现类
    /// </summary>
    /// <remarks>
    /// 构造函数
    /// </remarks>
    /// <param name="workerRepository"></param>
    /// <param name="photoRepository"></param>
    /// <param name="educationRepository"></param>
    /// <param name="nationRepository"></param>
    /// <param name="deptRepository"></param>
    /// <param name="positionRepository"></param>
    /// <param name="passportTypeRepository"></param>
    /// <param name="jWTHelper"></param>
    /// <param name="mailHelper"></param>
    /// <param name="logger"></param>
    public class EmployeeService(GenericRepository<Domain.Employee> workerRepository, GenericRepository<EmployeePhoto> photoRepository, GenericRepository<Education> educationRepository, GenericRepository<Nation> nationRepository, GenericRepository<Department> deptRepository, GenericRepository<Position> positionRepository, GenericRepository<PassportType> passportTypeRepository, JWTHelper jWTHelper, MailHelper mailHelper, DataProtectionHelper dataProtector, ITwoFactorAuthService twoFactorAuthService, ILogger<EmployeeService> logger) : IEmployeeService
    {

        /// <summary>
        /// 修改员工信息
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdateEmployee(UpdateEmployeeInputDto updateEmployeeInputDto)
        {
            try
            {
                //加密联系方式
                var sourceTelStr = string.Empty;
                if (!updateEmployeeInputDto.PhoneNumber.IsNullOrEmpty())
                {
                    sourceTelStr = dataProtector.EncryptEmployeeData(updateEmployeeInputDto.PhoneNumber);
                }
                //加密身份证
                var sourceIdStr = string.Empty;
                if (!updateEmployeeInputDto.IdCardNumber.IsNullOrEmpty())
                {
                    sourceIdStr = dataProtector.EncryptEmployeeData(updateEmployeeInputDto.IdCardNumber);
                }
                updateEmployeeInputDto.PhoneNumber = sourceTelStr;
                updateEmployeeInputDto.IdCardNumber = sourceIdStr;

                var password = workerRepository.GetFirst(a => a.EmployeeId == updateEmployeeInputDto.EmployeeId).Password;
                updateEmployeeInputDto.Password = password;

                workerRepository.Update(EntityMapper.Map<UpdateEmployeeInputDto, Domain.Employee>(updateEmployeeInputDto));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating employee information for EmployeeId: {EmployeeId}", updateEmployeeInputDto.EmployeeId);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();

        }

        /// <summary>
        /// 员工账号禁/启用
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        public BaseResponse ManagerEmployeeAccount(UpdateEmployeeInputDto updateEmployeeInputDto)
        {
            try
            {
                workerRepository.Update(a => new Domain.Employee()
                {
                    IsEnable = updateEmployeeInputDto.IsEnable,
                }, a => a.EmployeeId == updateEmployeeInputDto.EmployeeId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error managing employee account for EmployeeId: {EmployeeId}", updateEmployeeInputDto.EmployeeId);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 添加员工信息
        /// </summary>
        /// <param name="createEmployeeInputDto"></param>
        /// <returns></returns>
        public BaseResponse AddEmployee(CreateEmployeeInputDto createEmployeeInputDto)
        {
            try
            {
                //加密联系方式
                var sourceTelStr = string.Empty;
                if (!createEmployeeInputDto.PhoneNumber.IsNullOrEmpty())
                {
                    sourceTelStr = dataProtector.EncryptEmployeeData(createEmployeeInputDto.PhoneNumber);
                }
                //加密身份证
                var sourceIdStr = string.Empty;
                if (!createEmployeeInputDto.IdCardNumber.IsNullOrEmpty())
                {
                    sourceIdStr = dataProtector.EncryptEmployeeData(createEmployeeInputDto.IdCardNumber);
                }
                // 加密密码
                var sourcePwdStr = string.Empty;
                var newPassword = new RandomStringGenerator().GenerateSecurePassword();
                sourcePwdStr = dataProtector.EncryptEmployeeData(newPassword);

                var emailTemplate = EmailTemplate.GetNewRegistrationTemplate(createEmployeeInputDto.Name, newPassword);
                var result = mailHelper.SendMail(new List<string> { createEmployeeInputDto.EmailAddress }, emailTemplate.Subject, emailTemplate.Body, new List<string> { createEmployeeInputDto.EmailAddress });
                if (!result)
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("E-Mail Config Invaild, Add Employee Faild", "电子邮件配置无效，添加员工失败"), Code = BusinessStatusCode.InternalServerError };
                }

                createEmployeeInputDto.PhoneNumber = sourceTelStr;
                createEmployeeInputDto.IdCardNumber = sourceIdStr;
                createEmployeeInputDto.Password = sourcePwdStr;

                workerRepository.Insert(EntityMapper.Map<CreateEmployeeInputDto, Domain.Employee>(createEmployeeInputDto));

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding employee: {Message}", ex.Message);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 获取所有工作人员信息
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadEmployeeOutputDto> SelectEmployeeAll(ReadEmployeeInputDto readEmployeeInputDto)
        {
            readEmployeeInputDto ??= new ReadEmployeeInputDto();

            var where = SqlFilterBuilder.BuildExpression<Domain.Employee, ReadEmployeeInputDto>(readEmployeeInputDto, nameof(Domain.Employee.HireDate));
            var query = workerRepository.AsQueryable();
            var whereExpression = where.ToExpression();
            if (whereExpression != null)
            {
                query = query.Where(whereExpression);
            }

            query = query.OrderBy(a => a.EmployeeId);

            var count = 0;
            List<Domain.Employee> employees;
            if (!readEmployeeInputDto.IgnorePaging)
            {
                var page = readEmployeeInputDto.Page > 0 ? readEmployeeInputDto.Page : 1;
                var pageSize = readEmployeeInputDto.PageSize > 0 ? readEmployeeInputDto.PageSize : 15;
                employees = query.ToPageList(page, pageSize, ref count);
            }
            else
            {
                employees = query.ToList();
                count = employees.Count;
            }

            var educations = educationRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.EducationNumber)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.EducationName ?? "");
            var nations = nationRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.NationNumber)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.NationName ?? "");
            var departments = deptRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.DepartmentNumber)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.DepartmentName ?? "");
            var positions = positionRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.PositionNumber)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.PositionName ?? "");
            var passports = passportTypeRepository.GetList(a => a.IsDelete != 1)
                .GroupBy(a => a.PassportId)
                .ToDictionary(g => g.Key, g => g.FirstOrDefault()?.PassportName ?? "");

            var genderMap = Enum.GetValues(typeof(GenderType))
                .Cast<GenderType>()
                .ToDictionary(e => (int)e, e => EnumHelper.GetEnumDescription(e) ?? "");
            var politicalAffiliationMap = Enum.GetValues(typeof(PoliticalAffiliation))
                .Cast<PoliticalAffiliation>()
                .ToDictionary(e => e.ToString(), e => EnumHelper.GetEnumDescription(e) ?? "", StringComparer.OrdinalIgnoreCase);

            List<ReadEmployeeOutputDto> data;
            var useParallelProjection = readEmployeeInputDto.IgnorePaging && employees.Count >= 200;
            if (useParallelProjection)
            {
                var dtoArray = new ReadEmployeeOutputDto[employees.Count];
                System.Threading.Tasks.Parallel.For(0, employees.Count, i =>
                {
                    dtoArray[i] = MapToEmployeeOutputDto(employees[i], genderMap, nations, departments, positions, passports, politicalAffiliationMap, educations);
                });
                data = dtoArray.ToList();
            }
            else
            {
                data = employees.Select(source => MapToEmployeeOutputDto(source, genderMap, nations, departments, positions, passports, politicalAffiliationMap, educations)).ToList();
            }

            return new ListOutputDto<ReadEmployeeOutputDto>
            {
                Data = new PagedData<ReadEmployeeOutputDto>
                {
                    Items = data,
                    TotalCount = count
                }
            };
        }

        private ReadEmployeeOutputDto MapToEmployeeOutputDto(Domain.Employee source, Dictionary<int, string> genderMap, Dictionary<string, string> nations, Dictionary<string, string> departments, Dictionary<string, string> positions, Dictionary<int, string> passports, Dictionary<string, string> politicalAffiliationMap, Dictionary<string, string> educations)
        {
            return new ReadEmployeeOutputDto
            {
                Id = source.Id,
                EmployeeId = source.EmployeeId,
                Name = source.Name,
                Gender = source.Gender,
                GenderName = genderMap.TryGetValue(source.Gender, out var genderName) ? genderName : "",
                DateOfBirth = source.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                Ethnicity = source.Ethnicity,
                EthnicityName = nations.TryGetValue(source.Ethnicity, out var ethnicityName) ? ethnicityName : "",
                PhoneNumber = dataProtector.SafeDecryptEmployeeData(source.PhoneNumber),
                Department = source.Department,
                DepartmentName = departments.TryGetValue(source.Department, out var departmentName) ? departmentName : "",
                Address = source.Address ?? "",
                Position = source.Position,
                PositionName = positions.TryGetValue(source.Position, out var positionName) ? positionName : "",
                IdCardType = source.IdCardType,
                IdCardTypeName = passports.TryGetValue(source.IdCardType, out var passportName) ? passportName : "",
                IdCardNumber = dataProtector.SafeDecryptEmployeeData(source.IdCardNumber),
                HireDate = source.HireDate.ToDateTime(TimeOnly.MinValue),
                PoliticalAffiliation = source.PoliticalAffiliation,
                PoliticalAffiliationName = politicalAffiliationMap.TryGetValue(source.PoliticalAffiliation ?? "", out var politicalAffiliationName) ? politicalAffiliationName : "",
                EducationLevel = source.EducationLevel,
                EducationLevelName = educations.TryGetValue(source.EducationLevel, out var educationName) ? educationName : "",
                IsEnable = source.IsEnable,
                IsInitialize = source.IsInitialize,
                Password = source.Password,
                EmailAddress = source.EmailAddress,
                PhotoUrl = string.Empty,
                DataInsUsr = source.DataInsUsr,
                DataInsDate = source.DataInsDate,
                DataChgUsr = source.DataChgUsr,
                DataChgDate = source.DataChgDate,
                RowVersion = source.RowVersion,
                IsDelete = source.IsDelete
            };
        }

        /// <summary>
        /// 根据登录名称查询员工信息
        /// </summary>
        /// <param name="readEmployeeInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadEmployeeOutputDto> SelectEmployeeInfoByEmployeeId(ReadEmployeeInputDto readEmployeeInputDto)
        {
            var genders = Enum.GetValues(typeof(GenderType))
                .Cast<GenderType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();
            var w = workerRepository.GetFirst(a => a.EmployeeId == readEmployeeInputDto.EmployeeId);

            var source = EntityMapper.Map<Domain.Employee, ReadEmployeeOutputDto>(w);

            //解密身份证号码
            var sourceStr = w.IdCardNumber.IsNullOrEmpty() ? "" : dataProtector.SafeDecryptEmployeeData(w.IdCardNumber);
            source.IdCardNumber = sourceStr;
            //解密联系方式
            var sourceTelStr = w.PhoneNumber.IsNullOrEmpty() ? "" : dataProtector.SafeDecryptEmployeeData(w.PhoneNumber);
            source.PhoneNumber = sourceTelStr;
            //性别类型
            var genderType = genders.SingleOrDefault(a => a.Id == w.Gender);
            source.GenderName = genderType.Description.IsNullOrEmpty() ? "" : genderType.Description;
            //教育程度
            var eduction = educationRepository.GetFirst(a => a.EducationNumber == w.EducationLevel);
            source.EducationLevelName = eduction.EducationName.IsNullOrEmpty() ? "" : eduction.EducationName;
            //民族类型
            var nation = nationRepository.GetFirst(a => a.NationNumber == w.Ethnicity);
            source.EthnicityName = nation.NationName.IsNullOrEmpty() ? "" : nation.NationName;
            //部门
            var dept = deptRepository.GetFirst(a => a.DepartmentNumber == w.Department);
            source.DepartmentName = dept.DepartmentName.IsNullOrEmpty() ? "" : dept.DepartmentName;
            //职位
            var position = positionRepository.GetFirst(a => a.PositionNumber == w.Position);
            source.PositionName = position.PositionName.IsNullOrEmpty() ? "" : position.PositionName;
            var passport = passportTypeRepository.GetFirst(a => a.PassportId == w.IdCardType);
            source.IdCardTypeName = passport.IsNullOrEmpty() ? "" : passport.PassportName;
            //面貌
            source.PoliticalAffiliationName = EnumHelper.GetDescriptionByName<PoliticalAffiliation>(w.PoliticalAffiliation);

            var employeePhoto = photoRepository.GetFirst(a => a.EmployeeId.Equals(source.EmployeeId));
            if (employeePhoto != null && !string.IsNullOrEmpty(employeePhoto.PhotoPath))
                source.PhotoUrl = employeePhoto.PhotoPath ?? string.Empty;

            return new SingleOutputDto<ReadEmployeeOutputDto> { Data = source };
        }

        /// <summary>
        /// 员工端登录
        /// </summary>
        /// <param name="employeeLoginDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadEmployeeOutputDto> EmployeeLogin(EmployeeLoginDto employeeLoginDto)
        {
            Domain.Employee w = new Domain.Employee();
            var genders = Enum.GetValues(typeof(GenderType))
                .Cast<GenderType>()
                .Select(e => new EnumDto
                {
                    Id = (int)e,
                    Name = e.ToString(),
                    Description = EnumHelper.GetEnumDescription(e)
                })
                .ToList();
            w = workerRepository.GetFirst(a => a.EmployeeId == employeeLoginDto.EmployeeId || a.EmailAddress == employeeLoginDto.EmailAddress);
            if (w == null)
            {
                w = null;
                return new SingleOutputDto<ReadEmployeeOutputDto> { Code = BusinessStatusCode.BadRequest, Data = null, Message = LocalizationHelper.GetLocalizedString("Employee does not exist or entered incorrectly", "员工不存在或输入有误") };
            }
            var correctPassword = dataProtector.CompareEmployeeData(w.Password, employeeLoginDto.Password);

            if (!correctPassword)
            {
                return new SingleOutputDto<ReadEmployeeOutputDto> { Code = BusinessStatusCode.BadRequest, Data = null, Message = LocalizationHelper.GetLocalizedString("Invalid account or password", "账号或密码错误") };
            }

            var usedRecoveryCode = false;
            if (twoFactorAuthService.RequiresTwoFactor(TwoFactorUserType.Employee, w.Id))
            {
                if (string.IsNullOrWhiteSpace(employeeLoginDto.TwoFactorCode))
                {
                    return new SingleOutputDto<ReadEmployeeOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("2FA code is required", "需要输入2FA验证码"),
                        Data = new ReadEmployeeOutputDto
                        {
                            EmployeeId = w.EmployeeId,
                            Name = w.Name,
                            RequiresTwoFactor = true
                        }
                    };
                }

                var passed = twoFactorAuthService.VerifyLoginCode(TwoFactorUserType.Employee, w.Id, employeeLoginDto.TwoFactorCode, out usedRecoveryCode);
                if (!passed)
                {
                    return new SingleOutputDto<ReadEmployeeOutputDto>
                    {
                        Code = BusinessStatusCode.Unauthorized,
                        Message = LocalizationHelper.GetLocalizedString("Invalid 2FA code", "2FA验证码错误"),
                        Data = new ReadEmployeeOutputDto
                        {
                            EmployeeId = w.EmployeeId,
                            Name = w.Name,
                            RequiresTwoFactor = true
                        }
                    };
                }
            }

            w.Password = "";

            var output = EntityMapper.Map<Domain.Employee, ReadEmployeeOutputDto>(w);

            //性别类型
            var genderType = genders.SingleOrDefault(a => a.Id == w.Gender);
            output.GenderName = genderType.Description.IsNullOrEmpty() ? "" : genderType.Description;
            //教育程度
            var eduction = educationRepository.GetFirst(a => a.EducationNumber == w.EducationLevel);
            output.EducationLevelName = eduction.EducationName.IsNullOrEmpty() ? "" : eduction.EducationName;
            //民族类型
            var nation = nationRepository.GetFirst(a => a.NationNumber == w.Ethnicity);
            output.EthnicityName = nation.NationName.IsNullOrEmpty() ? "" : nation.NationName;
            //部门
            var dept = deptRepository.GetFirst(a => a.DepartmentNumber == w.Department);
            output.DepartmentName = dept.DepartmentName.IsNullOrEmpty() ? "" : dept.DepartmentName;
            //职位
            var position = positionRepository.GetFirst(a => a.PositionNumber == w.Position);
            output.PositionName = position.PositionName.IsNullOrEmpty() ? "" : position.PositionName;

            output.UserToken = jWTHelper.GenerateJWT(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, w.Name),
                new Claim(ClaimTypes.SerialNumber, w.EmployeeId)
            }));
            output.RefreshToken = jWTHelper.GenerateRefreshToken(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.SerialNumber, w.EmployeeId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }));
            output.RequiresTwoFactor = false;
            output.UsedRecoveryCodeLogin = usedRecoveryCode;
            if (usedRecoveryCode)
            {
                NotifyRecoveryCodeLoginByEmail(w.EmailAddress, w.Name, w.EmployeeId);
            }
            return new SingleOutputDto<ReadEmployeeOutputDto> { Data = output };
        }

        /// <summary>
        /// 备用码登录后邮件通知（员工）
        /// </summary>
        /// <param name="emailAddress">邮箱</param>
        /// <param name="employeeName">员工姓名</param>
        /// <param name="employeeId">员工工号</param>
        private void NotifyRecoveryCodeLoginByEmail(string? emailAddress, string? employeeName, string? employeeId)
        {
            if (string.IsNullOrWhiteSpace(emailAddress) || !MailAddress.TryCreate(emailAddress, out _))
            {
                logger.LogWarning("Recovery-code login alert skipped for employee {EmployeeId}: invalid email.", employeeId ?? string.Empty);
                return;
            }

            var recipient = emailAddress;
            var name = employeeName ?? "Employee";
            var identity = employeeId ?? string.Empty;
            _ = Task.Run(async () =>
            {
                await SendEmployeeRecoveryCodeAlertWithRetryAsync(recipient, name, identity);
            });
        }

        private async Task SendEmployeeRecoveryCodeAlertWithRetryAsync(string recipient, string employeeName, string employeeId)
        {
            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    var template = EmailTemplate.GetTwoFactorRecoveryCodeLoginAlertTemplate(employeeName, employeeId, DateTime.Now);
                    var sent = mailHelper.SendMail(new List<string> { recipient }, template.Subject, template.Body);
                    if (sent)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Recovery-code alert send failed for employee {EmployeeId}, attempt {Attempt}.", employeeId, attempt);
                }

                if (attempt < maxAttempts)
                {
                    await Task.Delay(TimeSpan.FromSeconds(attempt));
                }
            }

            logger.LogWarning("Recovery-code alert send exhausted retries for employee {EmployeeId}.", employeeId);
        }

        /// <summary>
        /// 修改员工账号密码
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdateEmployeeAccountPassword(UpdateEmployeeInputDto updateEmployeeInputDto)
        {
            try
            {
                var employee = workerRepository.GetFirst(a => a.EmployeeId == updateEmployeeInputDto.EmployeeId);

                if (employee.IsNullOrEmpty())
                {
                    return new BaseResponse()
                    {
                        Message = LocalizationHelper.GetLocalizedString("This employee does not exists", "员工不存在"),
                        Code = BusinessStatusCode.InternalServerError
                    };
                }

                var currentPassword = dataProtector.SafeDecryptEmployeeData(employee.Password);

                if (!updateEmployeeInputDto.OldPassword.Equals(currentPassword))
                {
                    return new BaseResponse()
                    {
                        Message = LocalizationHelper.GetLocalizedString("The old password is incorrect", "旧密码不正确"),
                        Code = BusinessStatusCode.InternalServerError
                    };
                }

                if (updateEmployeeInputDto.Password.Equals(currentPassword))
                {
                    return new BaseResponse()
                    {
                        Message = LocalizationHelper.GetLocalizedString("The new password cannot be the same as the old password", "新密码不能与旧密码相同"),
                        Code = BusinessStatusCode.InternalServerError
                    };
                }

                var newPwd = updateEmployeeInputDto.Password;
                string encrypted = dataProtector.EncryptEmployeeData(newPwd);

                if (!employee.EmailAddress.IsNullOrEmpty())
                {
                    var mailTemplate = EmailTemplate.GetUpdatePasswordTemplate(employee.Name, newPwd);
                    mailHelper.SendMail(new List<string> { employee.EmailAddress }, mailTemplate.Subject, mailTemplate.Body, new List<string> { employee.EmailAddress });
                }

                employee.Password = encrypted;
                employee.IsInitialize = 1;
                workerRepository.Update(employee);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating employee password for EmployeeId: {EmployeeId}", updateEmployeeInputDto.EmployeeId);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }

            return new BaseResponse();
        }

        /// <summary>
        /// 重置员工账号密码
        /// </summary>
        /// <param name="updateEmployeeInputDto"></param>
        /// <returns></returns>
        public BaseResponse ResetEmployeeAccountPassword(UpdateEmployeeInputDto updateEmployeeInputDto)
        {
            try
            {
                var newPwd = new RandomStringGenerator().GenerateSecurePassword();
                string encrypted = dataProtector.EncryptEmployeeData(newPwd);

                var employee = workerRepository.GetFirst(a => a.EmployeeId == updateEmployeeInputDto.EmployeeId);

                var emailAddress = employee.EmailAddress;

                if (emailAddress.IsNullOrEmpty())
                {
                    return new BaseResponse()
                    {
                        Message = LocalizationHelper.GetLocalizedString("No bound email address was found for the employee. Password reset cannot be completed."
                        , "未找到员工绑定的电子邮箱，无法重置密码。"),
                        Code = BusinessStatusCode.InternalServerError
                    };
                }

                var mailTemplate = EmailTemplate.GetResetPasswordTemplate(employee.Name, newPwd);

                var result = mailHelper.SendMail(new List<string> { emailAddress },
                    mailTemplate.Subject, mailTemplate.Body,
                    new List<string> { emailAddress });
                if (!result)
                {
                    return new BaseResponse { Message = LocalizationHelper.GetLocalizedString("E-Mail Config Invaild, Reset Password Faild", "电子邮件配置无效，重置密码失败"), Code = BusinessStatusCode.InternalServerError };
                }

                employee.Password = encrypted;

                workerRepository.Update(employee);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error resetting employee password for EmployeeId: {EmployeeId}", updateEmployeeInputDto.EmployeeId);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }

            return new BaseResponse();
        }

        /// <summary>
        /// 获取员工账号 2FA 状态
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorStatusOutputDto> GetTwoFactorStatus(string employeeSerialNumber)
        {
            return twoFactorAuthService.GetStatus(TwoFactorUserType.Employee, employeeSerialNumber);
        }

        /// <summary>
        /// 生成员工账号 2FA 绑定信息
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorSetupOutputDto> GenerateTwoFactorSetup(string employeeSerialNumber)
        {
            return twoFactorAuthService.GenerateSetup(TwoFactorUserType.Employee, employeeSerialNumber);
        }

        /// <summary>
        /// 启用员工账号 2FA
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> EnableTwoFactor(string employeeSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.Enable(TwoFactorUserType.Employee, employeeSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }

        /// <summary>
        /// 关闭员工账号 2FA
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码输入</param>
        /// <returns></returns>
        public BaseResponse DisableTwoFactor(string employeeSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.Disable(TwoFactorUserType.Employee, employeeSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }

        /// <summary>
        /// 重置员工账号恢复备用码
        /// </summary>
        /// <param name="employeeSerialNumber">员工工号（JWT SerialNumber）</param>
        /// <param name="inputDto">验证码或恢复备用码输入</param>
        /// <returns></returns>
        public SingleOutputDto<TwoFactorRecoveryCodesOutputDto> RegenerateTwoFactorRecoveryCodes(string employeeSerialNumber, TwoFactorCodeInputDto inputDto)
        {
            return twoFactorAuthService.RegenerateRecoveryCodes(TwoFactorUserType.Employee, employeeSerialNumber, inputDto?.VerificationCode ?? string.Empty);
        }
    }
}
