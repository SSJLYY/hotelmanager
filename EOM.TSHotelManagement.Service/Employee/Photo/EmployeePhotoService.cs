using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;
using jvncorelib.EntityLib;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 员工照片接口实现类
    /// </summary>
    public class EmployeePhotoService : IEmployeePhotoService
    {
        /// <summary>
        /// 员工照片
        /// </summary>
        private readonly GenericRepository<EmployeePhoto> workerPicRepository;

        /// <summary>
        /// 兰空图床帮助类
        /// </summary>
        private readonly LskyHelper lskyHelper;

        private readonly ILogger<EmployeePhotoService> logger;

        public EmployeePhotoService(GenericRepository<EmployeePhoto> workerPicRepository, LskyHelper lskyHelper, ILogger<EmployeePhotoService> logger)
        {
            this.workerPicRepository = workerPicRepository;
            this.lskyHelper = lskyHelper;
            this.logger = logger;
        }

        /// <summary>
        /// 查询员工照片
        /// </summary>
        /// <param name="readEmployeePhotoInputDto"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadEmployeePhotoOutputDto> EmployeePhoto(ReadEmployeePhotoInputDto readEmployeePhotoInputDto)
        {
            var workerPicData = new EmployeePhoto();

            workerPicData = workerPicRepository.GetFirst(a => a.EmployeeId.Equals(readEmployeePhotoInputDto.EmployeeId));

            if (workerPicData.IsNullOrEmpty())
            {
                return new SingleOutputDto<ReadEmployeePhotoOutputDto>
                {
                    Data = new ReadEmployeePhotoOutputDto
                    {
                        PhotoId = 0,
                        PhotoPath = "",
                        EmployeeId = readEmployeePhotoInputDto.EmployeeId
                    }
                };
            }

            return new SingleOutputDto<ReadEmployeePhotoOutputDto>
            {
                Data = EntityMapper.Map<EmployeePhoto, ReadEmployeePhotoOutputDto>(workerPicData)
            };
        }
        /// <summary>
        /// 添加员工照片
        /// </summary>
        /// <param name="createEmployeePhotoInputDto"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public SingleOutputDto<ReadEmployeePhotoOutputDto> InsertWorkerPhoto(CreateEmployeePhotoInputDto createEmployeePhotoInputDto, IFormFile file)
        {
            try
            {
                if (!lskyHelper.GetEnabledState().Result)
                {
                    return new SingleOutputDto<ReadEmployeePhotoOutputDto> { Message = LocalizationHelper.GetLocalizedString("Image upload service is not enabled", "图片上传服务未启用"), Code = BusinessStatusCode.BadRequest };
                }

                if (file == null || file.Length == 0)
                {
                    return new SingleOutputDto<ReadEmployeePhotoOutputDto> { Message = LocalizationHelper.GetLocalizedString("File cannot null", "文件不能为空"), Code = BusinessStatusCode.BadRequest };
                }

                if (file.Length > 1048576)
                {
                    return new SingleOutputDto<ReadEmployeePhotoOutputDto>
                    {
                        Message = LocalizationHelper.GetLocalizedString("Image size exceeds 1MB limit", "图片大小不能超过1MB"),
                        Code = BusinessStatusCode.BadRequest
                    };
                }
                if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
                {
                    return new SingleOutputDto<ReadEmployeePhotoOutputDto>
                    {
                        Message = LocalizationHelper.GetLocalizedString("Invalid image format", "图片格式不正确"),
                        Code = BusinessStatusCode.BadRequest
                    };
                }

                var token = lskyHelper.GetImageStorageTokenAsync().Result;
                if (string.IsNullOrEmpty(token))
                {
                    return new SingleOutputDto<ReadEmployeePhotoOutputDto> { Message = LocalizationHelper.GetLocalizedString("Get Token Fail", "获取Token失败"), Code = BusinessStatusCode.InternalServerError };
                }

                using var stream = file.OpenReadStream();
                stream.Seek(0, SeekOrigin.Begin);
                string imageUrl = lskyHelper.UploadImageAsync(
                    fileStream: stream,
                    fileName: file.FileName,
                    contentType: file.ContentType,
                    token: token
                ).Result;
                if (string.IsNullOrEmpty(imageUrl))
                {
                    return new SingleOutputDto<ReadEmployeePhotoOutputDto> { Message = "图片上传失败", Code = BusinessStatusCode.InternalServerError };
                }

                var workerPicData = workerPicRepository.GetFirst(a => a.EmployeeId.Equals(createEmployeePhotoInputDto.EmployeeId));
                if (workerPicData.IsNullOrEmpty())
                {
                    workerPicRepository.Insert(new EmployeePhoto
                    {
                        EmployeeId = createEmployeePhotoInputDto.EmployeeId,
                        PhotoPath = imageUrl
                    });
                }
                else
                {
                    workerPicData = workerPicRepository.GetFirst(a => a.EmployeeId.Equals(createEmployeePhotoInputDto.EmployeeId));
                    workerPicData.PhotoPath = imageUrl;
                    var updateResult = workerPicRepository.Update(workerPicData);
                    if (!updateResult)
                    {
                        return new SingleOutputDto<ReadEmployeePhotoOutputDto>
                        {
                            Message = LocalizationHelper.GetLocalizedString("Data has been modified by another user. Please refresh and retry.", "数据已被其他用户修改，请刷新后重试。"),
                            Code = BusinessStatusCode.Conflict
                        };
                    }
                }

                return new SingleOutputDto<ReadEmployeePhotoOutputDto>
                {
                    Data = new ReadEmployeePhotoOutputDto
                    {
                        PhotoId = 0,
                        PhotoPath = imageUrl,
                        EmployeeId = createEmployeePhotoInputDto.EmployeeId
                    }
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inserting employee photo: {Message}", ex.Message);
                return new SingleOutputDto<ReadEmployeePhotoOutputDto> { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
        }

        /// <summary>
        /// 删除员工照片
        /// </summary>
        /// <param name="deleteEmployeePhotoInputDto"></param>
        /// <returns></returns>
        public BaseResponse DeleteWorkerPhoto(DeleteEmployeePhotoInputDto deleteEmployeePhotoInputDto)
        {
            try
            {
                workerPicRepository.Delete(a => a.EmployeeId.Equals(deleteEmployeePhotoInputDto.EmployeeId));
            }
            catch (Exception ex)
            {
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }

        /// <summary>
        /// 更新员工照片
        /// </summary>
        /// <param name="updateEmployeePhotoInputDto"></param>
        /// <returns></returns>
        public BaseResponse UpdateWorkerPhoto(UpdateEmployeePhotoInputDto updateEmployeePhotoInputDto)
        {
            try
            {
                var workerPicData = workerPicRepository.GetFirst(a => a.EmployeeId.Equals(updateEmployeePhotoInputDto.EmployeeId));
                if (workerPicData == null)
                {
                    return new BaseResponse(BusinessStatusCode.NotFound, LocalizationHelper.GetLocalizedString("Photo information not found", "照片信息不存在"));
                }
                workerPicData.PhotoPath = updateEmployeePhotoInputDto.PhotoUrl;
                workerPicData.RowVersion = updateEmployeePhotoInputDto.RowVersion ?? 0;
                var updateResult = workerPicRepository.Update(workerPicData);
                if (!updateResult)
                {
                    return BaseResponseFactory.ConcurrencyConflict();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating employee photo: {Message}", ex.Message);
                return new BaseResponse { Message = LocalizationHelper.GetLocalizedString(ex.Message, ex.Message), Code = BusinessStatusCode.InternalServerError };
            }
            return new BaseResponse();
        }
    }
}
