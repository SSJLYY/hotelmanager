using EOM.TSHotelManagement.Common;
using EOM.TSHotelManagement.Contract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;

namespace EOM.TSHotelManagement.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values.SelectMany(v => v.Errors)
                                                  .Select(e => e.ErrorMessage)
                                                  .ToList();

                var response = new BaseResponse
                {
                    Code = BusinessStatusCode.BadRequest,
                    Message = LocalizationHelper.GetLocalizedString($"Data validate failure: {string.Join("\n", errors)}", $"数据验证失败: {string.Join("\n", errors)}"),
                };

                context.Result = new BadRequestObjectResult(response);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
