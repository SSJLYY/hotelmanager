using EOM.TSHotelManagement.Contract;
using Microsoft.AspNetCore.Http;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 员工照片模块接口
    /// </summary>
    public interface IEmployeePhotoService
    {
        /// <summary>
        /// 查询员工照片
        /// </summary>
        /// <param name="readEmployeePhotoInputDto"></param>
        /// <returns></returns>
        SingleOutputDto<ReadEmployeePhotoOutputDto> EmployeePhoto(ReadEmployeePhotoInputDto readEmployeePhotoInputDto);
        /// <summary>
        /// 添加员工照片
        /// </summary>
        /// <param name="createEmployeePhotoInputDto"></param>
        /// <param name="formFile"></param>
        /// <returns></returns>
        SingleOutputDto<ReadEmployeePhotoOutputDto> InsertWorkerPhoto(CreateEmployeePhotoInputDto createEmployeePhotoInputDto, IFormFile formFile);
        /// <summary>
        /// 删除员工照片
        /// </summary>
        /// <param name="deleteEmployeePhotoInputDto"></param>
        /// <returns></returns>
        BaseResponse DeleteWorkerPhoto(DeleteEmployeePhotoInputDto deleteEmployeePhotoInputDto);
        /// <summary>
        /// 更新员工照片
        /// </summary>
        /// <param name="updateEmployeePhotoInputDto"></param>
        /// <returns></returns>
        BaseResponse UpdateWorkerPhoto(UpdateEmployeePhotoInputDto updateEmployeePhotoInputDto);
    }
}
