using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;

namespace EOM.TSHotelManagement.WebApi
{
    internal static class ClientApiGroups
    {
        public const string Web = "v1_Web";
        public const string Desktop = "v1_Desktop";
        public const string Mobile = "v1_Mobile";

        public static readonly string[] All = new[] { Web, Desktop, Mobile };
    }

    internal sealed class ClientApiGroupConvention : IApplicationModelConvention
    {
        private static readonly HashSet<string> MobileControllers = new(StringComparer.OrdinalIgnoreCase)
        {
            "CustomerAccount",
            "News"
        };

        private static readonly HashSet<string> DesktopEndpoints = new(StringComparer.OrdinalIgnoreCase)
        {
            "/Base/DeleteRewardPunishmentType",
            "/Base/InsertRewardPunishmentType",
            "/Base/SelectCustoTypeByTypeId",
            "/Base/SelectDept",
            "/Base/SelectDeptAllCanUse",
            "/Base/SelectEducation",
            "/Base/SelectGenderTypeAll",
            "/Base/SelectNation",
            "/Base/SelectPassPortTypeByTypeId",
            "/Base/SelectPosition",
            "/Base/SelectReserTypeAll",
            "/Base/SelectRewardPunishmentTypeAll",
            "/Base/SelectRewardPunishmentTypeAllCanUse",
            "/Base/SelectRewardPunishmentTypeByTypeId",
            "/Base/UpdateRewardPunishmentType",
            "/Customer/UpdCustomerTypeByCustoNo",
            "/CustomerAccount/Login",
            "/CustomerAccount/Register",
            "/Employee/UpdateEmployeeAccountPassword",
            "/EmployeeCheck/SelectWorkerCheckDaySumByEmployeeId",
            "/EmployeePhoto/DeleteWorkerPhoto",
            "/EmployeePhoto/EmployeePhoto",
            "/EmployeePhoto/UpdateWorkerPhoto",
            "/EnergyManagement/InsertEnergyManagementInfo",
            "/Login/RefreshCSRFToken",
            "/NavBar/AddNavBar",
            "/NavBar/DeleteNavBar",
            "/NavBar/NavBarList",
            "/NavBar/UpdateNavBar",
            "/News/AddNews",
            "/News/DeleteNews",
            "/News/News",
            "/News/SelectNews",
            "/News/UpdateNews",
            "/Notice/InsertNotice",
            "/Notice/SelectNoticeAll",
            "/Notice/SelectNoticeByNoticeNo",
            "/PromotionContent/SelectPromotionContents",
            "/RewardPunishment/AddRewardPunishment",
            "/Role/ReadRolePermissions",
            "/Room/DayByRoomNo",
            "/Room/SelectCanUseRoomAllByRoomState",
            "/Room/SelectFixingRoomAllByRoomState",
            "/Room/SelectNotClearRoomAllByRoomState",
            "/Room/SelectNotUseRoomAllByRoomState",
            "/Room/SelectReservedRoomAllByRoomState",
            "/Room/SelectRoomByRoomNo",
            "/Room/SelectRoomByRoomPrice",
            "/Room/SelectRoomByRoomState",
            "/Room/SelectRoomByTypeName",
            "/Room/UpdateRoomInfoWithReser",
            "/Room/UpdateRoomStateByRoomNo",
            "/RoomType/SelectRoomTypeByRoomNo",
            "/Sellthing/SelectSellThingByNameAndPrice",
            "/Spend/SelectSpendByRoomNo",
            "/Spend/SeletHistorySpendInfoAll",
            "/Spend/SumConsumptionAmount",
            "/Utility/AddLog",
            "/Utility/DeleteRequestlogByRange",
            "/VipRule/SelectVipRule"
        };

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                foreach (var action in controller.Actions)
                {
                    action.ApiExplorer.GroupName = ResolveGroupName(controller.ControllerName, action.ActionName);
                }
            }
        }

        private static string ResolveGroupName(string controllerName, string actionName)
        {
            if (MobileControllers.Contains(controllerName))
            {
                return ClientApiGroups.Mobile;
            }

            var endpoint = $"/{controllerName}/{actionName}";
            return DesktopEndpoints.Contains(endpoint) ? ClientApiGroups.Desktop : ClientApiGroups.Web;
        }
    }
}
