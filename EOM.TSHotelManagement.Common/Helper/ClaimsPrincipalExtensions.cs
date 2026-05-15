using System.Security.Claims;

namespace EOM.TSHotelManagement.Common
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserNumber(this ClaimsPrincipal? principal)
        {
            return principal?.FindFirst(ClaimTypes.SerialNumber)?.Value
                ?? principal?.FindFirst("serialnumber")?.Value
                ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? string.Empty;
        }
    }
}
