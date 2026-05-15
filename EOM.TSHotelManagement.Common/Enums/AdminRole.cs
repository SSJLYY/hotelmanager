using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace EOM.TSHotelManagement.Common
{
    public enum AdminRole
    {
        [Description("超级管理员")]
        SuperAdmin = 1,
        [Description("管理员")]
        Admin = 0,
    }
}
