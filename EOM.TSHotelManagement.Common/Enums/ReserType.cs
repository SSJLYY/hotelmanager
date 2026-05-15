using System.ComponentModel;

namespace EOM.TSHotelManagement.Common
{
    public enum ReserType
    {
        [Description("线下")]
        Offline = 1,

        [Description("应用程序")]
        App = 2,

        [Description("小程序")]
        Applet = 3,

        [Description("网页端")]
        Website = 4,

        [Description("其他")]
        Other = 5,
    }
}
