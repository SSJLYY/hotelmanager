using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 照片控制器
    /// </summary>
    public class EmployeePhotoController : ControllerBase
    {
        private readonly IEmployeePhotoService workerPicService;

        public EmployeePhotoController(IEmployeePhotoService workerPicService)
        {
            this.workerPicService = workerPicService;
        }

        /// <summary>
        /// 查询员工照片
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.ep")]
        [HttpGet]
        public SingleOutputDto<ReadEmployeePhotoOutputDto> EmployeePhoto([FromQuery] ReadEmployeePhotoInputDto inputDto)
        {
            return workerPicService.EmployeePhoto(inputDto);
        }

        /// <summary>
        /// 添加员工照片
        /// </summary>
        /// <param name="inputDto"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.iwp")]
        [HttpPost]
        public SingleOutputDto<ReadEmployeePhotoOutputDto> InsertWorkerPhoto([FromForm] CreateEmployeePhotoInputDto inputDto, IFormFile file)
        {
            return workerPicService.InsertWorkerPhoto(inputDto, file);
        }

        /// <summary>
        /// 删除员工照片
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.dwp")]
        [HttpPost]
        public BaseResponse DeleteWorkerPhoto([FromBody] DeleteEmployeePhotoInputDto inputDto)
        {
            return workerPicService.DeleteWorkerPhoto(inputDto);
        }

        /// <summary>
        /// 更新员工照片
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        [RequirePermission("staffmanagement.uwp")]
        [HttpPost]
        public BaseResponse UpdateWorkerPhoto([FromBody] UpdateEmployeePhotoInputDto inputDto)
        {
            return workerPicService.UpdateWorkerPhoto(inputDto);
        }
    }
}

