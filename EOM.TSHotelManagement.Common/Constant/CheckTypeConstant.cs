using EOM.TSHotelManagement.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Common;

public class CheckTypeConstant : CodeConstantBase<CheckTypeConstant>
{
    /// <summary>
    /// 客户端
    /// </summary>
    public static readonly CheckTypeConstant Client = new CheckTypeConstant("Client", "客户端");
    /// <summary>
    /// 网页端
    /// </summary>
    public static readonly CheckTypeConstant Web = new CheckTypeConstant("Web", "网页端");
    private CheckTypeConstant(string code, string description) : base(code, description)
    {

    }
}
