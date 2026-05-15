using EOM.TSHotelManagement.Contract;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 导航控件模块接口
    /// </summary>
    public interface INavBarService
    {
        /// <summary>
        /// 导航控件列表
        /// </summary>
        /// <returns></returns>
        ListOutputDto<ReadNavBarOutputDto> NavBarList();
        /// <summary>
        /// 添加导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseResponse AddNavBar(CreateNavBarInputDto input);
        /// <summary>
        /// 更新导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseResponse UpdateNavBar(UpdateNavBarInputDto input);
        /// <summary>
        /// 删除导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        BaseResponse DeleteNavBar(DeleteNavBarInputDto input);
    }
}