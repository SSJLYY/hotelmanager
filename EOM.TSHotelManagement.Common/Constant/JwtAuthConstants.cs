using System;
using System.Collections.Generic;
using System.Text;

namespace EOM.TSHotelManagement.Common
{
    public static class JwtAuthConstants
    {
        public const string AuthFailureReasonItemKey = "AuthFailureReason";
        public const string AuthFailureReasonTokenRevoked = "token_revoked";
        public const string AuthFailureReasonTokenExpired = "token_expired";
        public const string AuthFailureReasonTokenInvalid = "token_invalid";
        public const string JwtTokenUserIdItemKey = "JwtTokenUserId";
        public const string JwtTokenJtiItemKey = "JwtTokenJti";
    }
}
