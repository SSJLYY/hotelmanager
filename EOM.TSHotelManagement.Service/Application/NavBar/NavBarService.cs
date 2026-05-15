using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Data;
using EOM.TSHotelManagement.Domain;

namespace EOM.TSHotelManagement.Service
{
    /// <summary>
    /// 导航控件模块
    /// </summary>
    public class NavBarService : INavBarService
    {
        /// <summary>
        /// 导航控件
        /// </summary>
        private readonly GenericRepository<NavBar> navBarRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="navBarRepository"></param>
        public NavBarService(GenericRepository<NavBar> navBarRepository)
        {
            this.navBarRepository = navBarRepository;
        }

        /// <summary>
        /// 导航控件列表
        /// </summary>
        /// <returns></returns>
        public ListOutputDto<ReadNavBarOutputDto> NavBarList()
        {
            var navBars = navBarRepository.GetList();
            var result = EntityMapper.MapList<NavBar, ReadNavBarOutputDto>(navBars);

            return new ListOutputDto<ReadNavBarOutputDto>
            {
                Data = new PagedData<ReadNavBarOutputDto>
                {
                    Items = result,
                    TotalCount = result.Count
                }
            };
        }

        /// <summary>
        /// 添加导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BaseResponse AddNavBar(CreateNavBarInputDto input)
        {
            var navBar = new NavBar
            {
                NavigationBarEvent = input.NavigationBarEvent,
                NavigationBarImage = input.NavigationBarImage,
                NavigationBarName = input.NavigationBarName,
                NavigationBarOrder = input.NavigationBarOrder,
                MarginLeft = input.MarginLeft,
                DataInsUsr = input.DataInsUsr,
                DataInsDate = input.DataInsDate
            };
            var result = navBarRepository.Insert(navBar);
            if (!result)
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = "添加失败"
                };
            }
            return new BaseResponse
            {
                Code = BusinessStatusCode.Success,
                Message = "添加成功"
            };
        }

        /// <summary>
        /// 更新导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BaseResponse UpdateNavBar(UpdateNavBarInputDto input)
        {
            var navBar = navBarRepository.GetById(input.NavigationBarId);
            if (navBar == null)
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = "导航控件未找到"
                };
            }
            navBar.NavigationBarName = input.NavigationBarName;
            navBar.NavigationBarOrder = input.NavigationBarOrder;
            navBar.NavigationBarImage = input.NavigationBarImage;
            navBar.NavigationBarEvent = input.NavigationBarEvent;
            navBar.MarginLeft = input.MarginLeft;
            navBar.RowVersion = input.RowVersion ?? 0;
            var result = navBarRepository.Update(navBar);
            if (!result)
            {
                return BaseResponseFactory.ConcurrencyConflict();
            }
            return new BaseResponse
            {
                Code = BusinessStatusCode.Success,
                Message = "更新成功"
            };
        }

        /// <summary>
        /// 删除导航控件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public BaseResponse DeleteNavBar(DeleteNavBarInputDto input)
        {
            // 当DelIds为空列表时，表示删除所有导航条目（全局配置）
            if (input?.DelIds == null || !input.DelIds.Any())
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString("Parameters Invalid", "参数错误")
                };
            }

            var delIds = DeleteConcurrencyHelper.GetDeleteIds(input);
            var navBars = navBarRepository.GetList(a => delIds.Contains(a.Id));

            if (!navBars.Any())
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.NotFound,
                    Message = LocalizationHelper.GetLocalizedString("Navigation Control Not Found", "导航控件未找到")
                };
            }

            if (DeleteConcurrencyHelper.HasDeleteConflict(input, navBars, a => a.Id, a => a.RowVersion))
            {
                return BaseResponseFactory.ConcurrencyConflict();
            }

            // 批量软删除
            var result = navBarRepository.SoftDeleteRange(navBars);

            if (!result)
            {
                return new BaseResponse
                {
                    Code = BusinessStatusCode.InternalServerError,
                    Message = LocalizationHelper.GetLocalizedString("Delete failure", "删除失败")
                };
            }

            return new BaseResponse
            {
                Code = BusinessStatusCode.Success,
                Message = LocalizationHelper.GetLocalizedString("Delete successful", "删除成功")
            };
        }
    }
}
