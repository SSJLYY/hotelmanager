using System;

namespace EOM.TSHotelManagement.WebApi.Authorization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class BusinessOperationAuditAttribute : Attribute
    {
    }
}
