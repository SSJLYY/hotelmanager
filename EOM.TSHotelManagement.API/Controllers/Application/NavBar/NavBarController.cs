using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 导航控件模块控制器
    /// </summary>
    public class NavBarController : ControllerBase
    {
        /// <summary>
        /// 导航控件
        /// </summary>
        private readonly INavBarService navBarService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navBarService"></param>
        public NavBarController(INavBarService navBarService)
        {
            this.navBarService = navBarService;
        }

        /// <summary>
        /// 导航控件列表
        /// </summary>
        /// <returns></returns>
        [RequirePermission("navbar.navbarlist")]
        [HttpGet]
        public ListOutputDto<ReadNavBarOutputDto> NavBarList()
        {
            return navBarService.NavBarList();
        }
        /// <summary>
        /// 添加导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequirePermission("navbar.addnavbar")]
        [HttpPost]
        public BaseResponse AddNavBar([FromBody] CreateNavBarInputDto input)
        {
            return navBarService.AddNavBar(input);
        }
        /// <summary>
        /// 更新导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequirePermission("navbar.un")]
        [HttpPost]
        public BaseResponse UpdateNavBar([FromBody] UpdateNavBarInputDto input)
        {
            return navBarService.UpdateNavBar(input);
        }
        /// <summary>
        /// 删除导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [RequirePermission("navbar.dn")]
        [HttpPost]
        public BaseResponse DeleteNavBar([FromBody] DeleteNavBarInputDto input)
        {
            return navBarService.DeleteNavBar(input);
        }

    }
}
