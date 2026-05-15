using EOM.TSHotelManagement.Contract;
using EOM.TSHotelManagement.Service;
using EOM.TSHotelManagement.WebApi.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EOM.TSHotelManagement.WebApi.Controllers
{
    /// <summary>
    /// 用户信息控制器
    /// </summary>
    [BusinessOperationAudit]
    public class CustomerController : ControllerBase
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        private readonly ICustomerService customerService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerService"></param>
        public CustomerController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        /// <summary>
        /// 添加客户信息
        /// </summary>
        /// <param name="custo"></param>
        /// <returns></returns>
        [RequirePermission("customer.ici")]
        [HttpPost]
        public BaseResponse InsertCustomerInfo([FromBody] CreateCustomerInputDto custo)
        {
            return customerService.InsertCustomerInfo(custo);
        }

        /// <summary>
        /// 更新客户信息
        /// </summary>
        /// <param name="custo"></param>
        /// <returns></returns>
        [RequirePermission("customer.uci")]
        [HttpPost]
        public BaseResponse UpdCustomerInfo([FromBody] UpdateCustomerInputDto custo)
        {
            return customerService.UpdCustomerInfo(custo);
        }

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="custo"></param>
        /// <returns></returns>
        [RequirePermission("customer.dci")]
        [HttpPost]
        public BaseResponse DelCustomerInfo([FromBody] DeleteCustomerInputDto custo)
        {
            return customerService.DelCustomerInfo(custo);
        }

        /// <summary>
        /// 更新客户类型(即会员等级)
        /// </summary>
        /// <param name="updateCustomerInputDto"></param>
        /// <returns></returns>
        [RequirePermission("customer.uctbcn")]
        [HttpPost]
        public BaseResponse UpdCustomerTypeByCustoNo([FromBody] UpdateCustomerInputDto updateCustomerInputDto)
        {
            return customerService.UpdCustomerTypeByCustoNo(updateCustomerInputDto);
        }

        /// <summary>
        /// 查询所有客户信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("customer.scs")]
        [HttpGet]
        public ListOutputDto<ReadCustomerOutputDto> SelectCustomers(ReadCustomerInputDto custo)
        {
            return customerService.SelectCustomers(custo);
        }

        /// <summary>
        /// 查询指定客户信息
        /// </summary>
        /// <returns></returns>
        [RequirePermission("customer.scbi")]
        [HttpGet]
        public SingleOutputDto<ReadCustomerOutputDto> SelectCustoByInfo([FromQuery] ReadCustomerInputDto custo)
        {
            return customerService.SelectCustoByInfo(custo);
        }

    }
}
