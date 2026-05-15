using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EOM.TSHotelManagement.Service
{
    public class ProfileService(
        GenericRepository<Administrator> adminRepository,
        GenericRepository<AdministratorPhoto> adminPhotoRepository,
        GenericRepository<AdministratorType> adminTypeRepository,
        GenericRepository<Domain.Employee> employeeRepository,
        GenericRepository<EmployeePhoto> employeePhotoRepository,
        GenericRepository<Department> departmentRepository,
        GenericRepository<Position> positionRepository,
        DataProtectionHelper dataProtectionHelper,
        LskyHelper lskyHelper,
        MailHelper mailHelper,
        ILogger<ProfileService> logger) : IProfileService
    {
        private const string AdminLoginType = "admin";
        private const string EmployeeLoginType = "employee";
        private readonly GenericRepository<Administrator> _adminRepository = adminRepository;
        private readonly GenericRepository<AdministratorPhoto> _adminPhotoRepository = adminPhotoRepository;
        private readonly GenericRepository<AdministratorType> _adminTypeRepository = adminTypeRepository;
        private readonly GenericRepository<Domain.Employee> _employeeRepository = employeeRepository;
        private readonly GenericRepository<EmployeePhoto> _employeePhotoRepository = employeePhotoRepository;
        private readonly GenericRepository<Department> _departmentRepository = departmentRepository;
        private readonly GenericRepository<Position> _positionRepository = positionRepository;
        private readonly DataProtectionHelper _dataProtectionHelper = dataProtectionHelper;
        private readonly LskyHelper _lskyHelper = lskyHelper;
        private readonly MailHelper _mailHelper = mailHelper;
        private readonly ILogger<ProfileService> _logger = logger;

        public SingleOutputDto<CurrentProfileOutputDto> GetCurrentProfile(string serialNumber)
        {
            try
            {
                var currentUser = ResolveCurrentUser(serialNumber);
                if (currentUser == null)
                {
                    return new SingleOutputDto<CurrentProfileOutputDto>
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Current profile not found", "未找到当前登录人资料")
                    };
                }

                return new SingleOutputDto<CurrentProfileOutputDto>
                {
                    Message = LocalizationHelper.GetLocalizedString("ok", "成功"),
                    Data = BuildCurrentProfileOutput(currentUser)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current profile for serial number: {SerialNumber}", serialNumber);
                return new SingleOutputDto<CurrentProfileOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message)
                };
            }
        }

        public async Task<SingleOutputDto<UploadAvatarOutputDto>> UploadAvatar(string serialNumber, UploadAvatarInputDto inputDto, IFormFile file)
        {
            try
            {
                var currentUser = ResolveCurrentUser(serialNumber);
                if (currentUser == null)
                {
                    return new SingleOutputDto<UploadAvatarOutputDto>
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Current profile not found", "未找到当前登录人资料")
                    };
                }

                if (!string.IsNullOrWhiteSpace(inputDto?.LoginType)
                    && !string.Equals(inputDto.LoginType, currentUser.LoginType, StringComparison.OrdinalIgnoreCase))
                {
                    return new SingleOutputDto<UploadAvatarOutputDto>
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("Login type does not match current user", "账号类型与当前登录人不匹配")
                    };
                }

                var uploadResult = await UploadImageAsync(file);
                if (!uploadResult.Success)
                {
                    return uploadResult;
                }

                var saveResult = SaveAvatarPath(currentUser, uploadResult.Data.PhotoUrl);
                if (!saveResult.Success)
                {
                    return saveResult;
                }

                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Message = LocalizationHelper.GetLocalizedString("ok", "成功"),
                    Data = new UploadAvatarOutputDto
                    {
                        PhotoUrl = uploadResult.Data.PhotoUrl
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading avatar for serial number: {SerialNumber}", serialNumber);
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message)
                };
            }
        }

        public BaseResponse ChangePassword(string serialNumber, ChangePasswordInputDto inputDto)
        {
            try
            {
                var currentUser = ResolveCurrentUser(serialNumber);
                if (currentUser == null)
                {
                    return new SingleOutputDto<object?>
                    {
                        Code = BusinessStatusCode.NotFound,
                        Message = LocalizationHelper.GetLocalizedString("Current profile not found", "未找到当前登录人资料")
                    };
                }

                if (!string.Equals(inputDto.NewPassword, inputDto.ConfirmPassword, StringComparison.Ordinal))
                {
                    return new SingleOutputDto<object?>
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("The new password and confirmation password do not match", "新密码与确认密码不一致")
                    };
                }

                if (string.Equals(inputDto.OldPassword, inputDto.NewPassword, StringComparison.Ordinal))
                {
                    return new SingleOutputDto<object?>
                    {
                        Code = BusinessStatusCode.BadRequest,
                        Message = LocalizationHelper.GetLocalizedString("The new password cannot be the same as the old password", "新密码不能与旧密码相同")
                    };
                }

                BaseResponse updateResponse = currentUser.LoginType switch
                {
                    AdminLoginType => ChangeAdminPassword(currentUser.Admin, inputDto),
                    EmployeeLoginType => ChangeEmployeePassword(currentUser.Employee, inputDto),
                    _ => new BaseResponse(BusinessStatusCode.BadRequest, LocalizationHelper.GetLocalizedString("Unsupported login type", "不支持的登录类型"))
                };

                if (!updateResponse.Success)
                {
                    return new SingleOutputDto<object?>
                    {
                        Code = updateResponse.Code,
                        Message = updateResponse.Message
                    };
                }

                return new SingleOutputDto<object?>
                {
                    Message = LocalizationHelper.GetLocalizedString("Password changed successfully", "密码修改成功"),
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for serial number: {SerialNumber}", serialNumber);
                return new SingleOutputDto<object?>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message),
                    Data = null
                };
            }
        }

        private CurrentProfileOutputDto BuildCurrentProfileOutput(CurrentUserProfile currentUser)
        {
            return string.Equals(currentUser.LoginType, AdminLoginType, StringComparison.OrdinalIgnoreCase)
                ? BuildAdminCurrentProfile(currentUser.Admin, currentUser.PhotoUrl)
                : BuildEmployeeCurrentProfile(currentUser.Employee, currentUser.PhotoUrl);
        }

        private CurrentProfileOutputDto BuildAdminCurrentProfile(Administrator admin, string photoUrl)
        {
            var account = FirstNonEmpty(admin.Account, admin.Number);
            var displayName = FirstNonEmpty(admin.Name, account);
            var typeCode = FirstNonEmpty(admin.Type, string.Empty);
            var typeName = FirstNonEmpty(
                _adminTypeRepository.GetFirst(a => a.TypeId == admin.Type && a.IsDelete != 1)?.TypeName,
                typeCode);

            return new CurrentProfileOutputDto
            {
                LoginType = AdminLoginType,
                UserNumber = FirstNonEmpty(admin.Number, account),
                Account = account,
                DisplayName = displayName,
                PhotoUrl = FirstNonEmpty(photoUrl, string.Empty),
                Profile = new CurrentProfileAdminDto
                {
                    Number = FirstNonEmpty(admin.Number, account),
                    Account = account,
                    Name = displayName,
                    TypeName = typeName,
                    Type = typeCode,
                    IsSuperAdmin = admin.IsSuperAdmin,
                    PhotoUrl = FirstNonEmpty(photoUrl, string.Empty)
                }
            };
        }

        private CurrentProfileOutputDto BuildEmployeeCurrentProfile(Domain.Employee employee, string photoUrl)
        {
            var account = FirstNonEmpty(employee.EmailAddress, employee.EmployeeId);
            var displayName = FirstNonEmpty(employee.Name, employee.EmployeeId);
            var departmentName = FirstNonEmpty(
                _departmentRepository.GetFirst(a => a.DepartmentNumber == employee.Department && a.IsDelete != 1)?.DepartmentName,
                employee.Department);
            var positionName = FirstNonEmpty(
                _positionRepository.GetFirst(a => a.PositionNumber == employee.Position && a.IsDelete != 1)?.PositionName,
                employee.Position);

            return new CurrentProfileOutputDto
            {
                LoginType = EmployeeLoginType,
                UserNumber = FirstNonEmpty(employee.EmployeeId, account),
                Account = account,
                DisplayName = displayName,
                PhotoUrl = FirstNonEmpty(photoUrl, string.Empty),
                Profile = new CurrentProfileEmployeeDto
                {
                    EmployeeId = FirstNonEmpty(employee.EmployeeId, account),
                    Name = displayName,
                    DepartmentName = departmentName,
                    PositionName = positionName,
                    PhoneNumber = FirstNonEmpty(dataProtectionHelper.SafeDecryptEmployeeData(employee.PhoneNumber), string.Empty),
                    EmailAddress = FirstNonEmpty(employee.EmailAddress, account),
                    Address = FirstNonEmpty(employee.Address, string.Empty),
                    HireDate = FormatDate(employee.HireDate),
                    DateOfBirth = FormatDate(employee.DateOfBirth),
                    PhotoUrl = FirstNonEmpty(photoUrl, string.Empty)
                }
            };
        }

        private BaseResponse ChangeAdminPassword(Administrator admin, ChangePasswordInputDto inputDto)
        {
            var currentPassword = _dataProtectionHelper.SafeDecryptAdministratorData(admin.Password);
            if (!string.Equals(inputDto.OldPassword, currentPassword, StringComparison.Ordinal))
            {
                return new BaseResponse(
                    BusinessStatusCode.BadRequest,
                    LocalizationHelper.GetLocalizedString("The old password is incorrect", "旧密码不正确"));
            }

            admin.Password = _dataProtectionHelper.EncryptAdministratorData(inputDto.NewPassword);
            var updateResult = _adminRepository.Update(admin);
            if (!updateResult)
            {
                return BaseResponseFactory.ConcurrencyConflict();
            }

            TrySendPasswordChangeEmail(admin.Account, admin.Name, inputDto.NewPassword);
            return new BaseResponse();
        }

        private BaseResponse ChangeEmployeePassword(Domain.Employee employee, ChangePasswordInputDto inputDto)
        {
            var currentPassword = _dataProtectionHelper.SafeDecryptEmployeeData(employee.Password);
            if (!string.Equals(inputDto.OldPassword, currentPassword, StringComparison.Ordinal))
            {
                return new BaseResponse(
                    BusinessStatusCode.BadRequest,
                    LocalizationHelper.GetLocalizedString("The old password is incorrect", "旧密码不正确"));
            }

            employee.Password = _dataProtectionHelper.EncryptEmployeeData(inputDto.NewPassword);
            employee.IsInitialize = 1;
            var updateResult = _employeeRepository.Update(employee);
            if (!updateResult)
            {
                return BaseResponseFactory.ConcurrencyConflict();
            }

            TrySendPasswordChangeEmail(employee.EmailAddress, employee.Name, inputDto.NewPassword);
            return new BaseResponse();
        }

        private async Task<SingleOutputDto<UploadAvatarOutputDto>> UploadImageAsync(IFormFile file)
        {
            if (!await _lskyHelper.GetEnabledState())
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Image upload service is not enabled", "图片上传服务未启用")
                };
            }

            if (file == null || file.Length == 0)
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("File cannot null", "文件不能为空")
                };
            }

            if (file.Length > 1048576)
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Image size exceeds 1MB limit", "图片大小不能超过1MB")
                };
            }

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Invalid image format", "图片格式不正确")
                };
            }

            using var stream = file.OpenReadStream();
            if (!TryDetectImageContentType(stream, out var detectedContentType))
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Invalid image format", "图片格式不正确")
                };
            }

            if (!IsAllowedDeclaredImageContentType(file.ContentType, detectedContentType))
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Invalid image format", "图片格式不正确")
                };
            }

            var token = await _lskyHelper.GetImageStorageTokenAsync();
            if (string.IsNullOrWhiteSpace(token))
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("Get Token Fail", "获取Token失败")
                };
            }

            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var imageUrl = await _lskyHelper.UploadImageAsync(
                fileStream: stream,
                fileName: file.FileName,
                contentType: detectedContentType,
                token: token);

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("Image upload failed", "图片上传失败")
                };
            }

            return new SingleOutputDto<UploadAvatarOutputDto>
            {
                Data = new UploadAvatarOutputDto
                {
                    PhotoUrl = imageUrl
                }
            };
        }

        private SingleOutputDto<UploadAvatarOutputDto> SaveAvatarPath(CurrentUserProfile currentUser, string photoUrl)
        {
            if (string.Equals(currentUser.LoginType, AdminLoginType, StringComparison.OrdinalIgnoreCase))
            {
                var adminPhoto = _adminPhotoRepository.GetFirst(a => a.AdminNumber == currentUser.UserNumber && a.IsDelete != 1);
                if (adminPhoto == null)
                {
                    _adminPhotoRepository.Insert(new AdministratorPhoto
                    {
                        AdminNumber = currentUser.UserNumber,
                        PhotoPath = photoUrl
                    });

                    return new SingleOutputDto<UploadAvatarOutputDto>();
                }

                adminPhoto.PhotoPath = photoUrl;
                if (!_adminPhotoRepository.Update(adminPhoto))
                {
                    return new SingleOutputDto<UploadAvatarOutputDto>
                    {
                        Code = BusinessStatusCode.Conflict,
                        Message = LocalizationHelper.GetLocalizedString("Data has been modified by another user. Please refresh and retry.", "数据已被其他用户修改，请刷新后重试。")
                    };
                }

                return new SingleOutputDto<UploadAvatarOutputDto>();
            }

            var employeePhoto = _employeePhotoRepository.GetFirst(a => a.EmployeeId == currentUser.UserNumber && a.IsDelete != 1);
            if (employeePhoto == null)
            {
                _employeePhotoRepository.Insert(new EmployeePhoto
                {
                    EmployeeId = currentUser.UserNumber,
                    PhotoPath = photoUrl
                });

                return new SingleOutputDto<UploadAvatarOutputDto>();
            }

            employeePhoto.PhotoPath = photoUrl;
            if (!_employeePhotoRepository.Update(employeePhoto))
            {
                return new SingleOutputDto<UploadAvatarOutputDto>
                {
                    Code = BusinessStatusCode.Conflict,
                    Message = LocalizationHelper.GetLocalizedString("Data has been modified by another user. Please refresh and retry.", "数据已被其他用户修改，请刷新后重试。")
                };
            }

            return new SingleOutputDto<UploadAvatarOutputDto>();
        }

        private CurrentUserProfile? ResolveCurrentUser(string serialNumber)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                return null;
            }

            var admin = _adminRepository.GetFirst(a => a.Number == serialNumber && a.IsDelete != 1);
            if (admin != null)
            {
                var adminPhoto = _adminPhotoRepository.GetFirst(a => a.AdminNumber == admin.Number && a.IsDelete != 1);
                return new CurrentUserProfile
                {
                    LoginType = AdminLoginType,
                    UserNumber = admin.Number,
                    Account = admin.Account,
                    DisplayName = admin.Name,
                    PhotoUrl = adminPhoto?.PhotoPath ?? string.Empty,
                    Admin = admin
                };
            }

            var employee = _employeeRepository.GetFirst(a => a.EmployeeId == serialNumber && a.IsDelete != 1);
            if (employee != null)
            {
                var employeePhoto = _employeePhotoRepository.GetFirst(a => a.EmployeeId == employee.EmployeeId && a.IsDelete != 1);
                return new CurrentUserProfile
                {
                    LoginType = EmployeeLoginType,
                    UserNumber = employee.EmployeeId,
                    Account = string.IsNullOrWhiteSpace(employee.EmailAddress) ? employee.EmployeeId : employee.EmailAddress,
                    DisplayName = employee.Name,
                    PhotoUrl = employeePhoto?.PhotoPath ?? string.Empty,
                    Employee = employee
                };
            }

            return null;
        }

        private void TrySendPasswordChangeEmail(string? emailAddress, string? displayName, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(emailAddress) || !MailAddress.TryCreate(emailAddress, out _))
            {
                return;
            }

            try
            {
                var template = EmailTemplate.GetUpdatePasswordTemplate(displayName ?? "User", newPassword);
                _mailHelper.SendMail(new List<string> { emailAddress }, template.Subject, template.Body, new List<string> { emailAddress });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Password change email send failed for account: {Account}", emailAddress);
            }
        }

        private static bool IsAllowedDeclaredImageContentType(string? declaredContentType, string detectedContentType)
        {
            if (string.IsNullOrWhiteSpace(declaredContentType))
            {
                return true;
            }

            return string.Equals(declaredContentType, detectedContentType, StringComparison.OrdinalIgnoreCase);
        }

        private static bool TryDetectImageContentType(Stream stream, out string detectedContentType)
        {
            detectedContentType = string.Empty;
            if (stream == null || !stream.CanRead)
            {
                return false;
            }

            var header = new byte[8];
            var bytesRead = stream.Read(header, 0, header.Length);
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var signature = header.AsSpan(0, bytesRead);
            if (signature.SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            {
                detectedContentType = "image/png";
                return true;
            }

            if (signature.Length >= 3
                && signature[0] == 0xFF
                && signature[1] == 0xD8
                && signature[2] == 0xFF)
            {
                detectedContentType = "image/jpeg";
                return true;
            }

            return false;
        }

        private static string FirstNonEmpty(string? primary, string? fallback)
        {
            if (!string.IsNullOrWhiteSpace(primary))
            {
                return primary;
            }

            return fallback ?? string.Empty;
        }

        private static string FormatDate(DateOnly date)
        {
            return date == default ? string.Empty : date.ToString("yyyy-MM-dd");
        }

        private sealed class CurrentUserProfile
        {
            public string LoginType { get; set; }
            public string UserNumber { get; set; }
            public string Account { get; set; }
            public string DisplayName { get; set; }
            public string PhotoUrl { get; set; }
            public Administrator Admin { get; set; }
            public Domain.Employee Employee { get; set; }
        }
    }
}
