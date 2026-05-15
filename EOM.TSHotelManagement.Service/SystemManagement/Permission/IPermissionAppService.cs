using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 权限服务接口
    /// </summary>
    public interface IPermissionAppService
    {
        /// <summary>
        /// 查询权限列表（支持条件过滤与分页/忽略分页）
        /// </summary>
        /// <param name="input">查询条件</param>
        /// <returns>权限列表</returns>
        ListOutputDto<ReadPermissionOutputDto> SelectPermissionList(ReadPermissionInputDto input);
    }
}