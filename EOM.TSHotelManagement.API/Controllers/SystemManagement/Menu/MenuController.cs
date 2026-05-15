using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// </summary>
    public class MenuController : ControllerBase
    {
        private readonly IMenuService menuService;

        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService;
        }

        /// <summary>
        /// 查询所有菜单信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("menumanagement.sma")]
        [HttpGet]
        public ListOutputDto<ReadMenuOutputDto> SelectMenuAll(ReadMenuInputDto readMenuInputDto)
        {
            return menuService.SelectMenuAll(readMenuInputDto);
        }

        /// <summary>
        /// 构建菜单树
        /// </summary>
        /// <returns></returns>
        [RequirePermission("menumanagement.bma")]
        [HttpPost]
        public ListOutputDto<MenuDto> BuildMenuAll([FromBody] BaseInputDto baseInputDto)
        {
            return menuService.BuildMenuAll(baseInputDto);
        }

        /// <summary>
        /// 插入菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [RequirePermission("menumanagement.insertmenu")]
        [HttpPost]
        public BaseResponse InsertMenu([FromBody] CreateMenuInputDto menu)
        {
            return menuService.InsertMenu(menu);
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [RequirePermission("menumanagement.updatemenu")]
        [HttpPost]
        public BaseResponse UpdateMenu([FromBody] UpdateMenuInputDto menu)
        {
            return menuService.UpdateMenu(menu);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        [RequirePermission("menumanagement.deletemenu")]
        [HttpPost]
        public BaseResponse DeleteMenu([FromBody] DeleteMenuInputDto menu)
        {
            return menuService.DeleteMenu(menu);
        }
    }
}
