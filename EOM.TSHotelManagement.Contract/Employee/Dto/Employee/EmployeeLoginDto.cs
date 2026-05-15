using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Contract
{
    public class EmployeeLoginDto: BaseDto
    {
        public string EmployeeId { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string? TwoFactorCode { get; set; }
    }
}
