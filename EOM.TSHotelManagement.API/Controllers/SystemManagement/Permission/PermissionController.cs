using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 权限控制器
    /// </summary>
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionAppService _permissionAppService;

        public PermissionController(IPermissionAppService permissionAppService)
        {
            _permissionAppService = permissionAppService;
        }

        /// <summary>
        /// 查询权限列表（支持条件过滤与分页/忽略分页）
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>权限列表</returns>
        [RequirePermission("system:user:assign.spl")]
        [HttpPost]
        public ListOutputDto<ReadPermissionOutputDto> SelectPermissionList([FromBody] ReadPermissionInputDto input)
        {
            return _permissionAppService.SelectPermissionList(input);
        }
    }
}
