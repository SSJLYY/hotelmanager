using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace EOM.TSHotelManagement.Common
{
    public class DataProtectionHelper
    {
        private const string DataProtectionPayloadPrefix = "CfDJ8";
        private readonly IDataProtector _employeeProtector;
        private readonly IDataProtector _reservationProtector;
        private readonly IDataProtector _customerProtector;
        private readonly IDataProtector _adminProtector;
        private readonly IDataProtector _twoFactorProtector;

        public DataProtectionHelper(IDataProtectionProvider dataProtectionProvider)
        {
            _employeeProtector = dataProtectionProvider.CreateProtector("EmployeeInfoProtector");
            _reservationProtector = dataProtectionProvider.CreateProtector("ReservationInfoProtector");
            _customerProtector = dataProtectionProvider.CreateProtector("CustomerInfoProtector");
            _adminProtector = dataProtectionProvider.CreateProtector("AdminInfoProtector");
            _twoFactorProtector = dataProtectionProvider.CreateProtector("TwoFactorProtector");
        }

        private string DecryptData(string encryptedData, IDataProtector protector)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return encryptedData;

            // Fast path: ASP.NET Core DataProtection payloads usually start with "CfDJ8".
            // This avoids expensive exception-based fallback on legacy/plaintext values.
            if (LooksLikeDataProtectionPayload(encryptedData))
            {
                try
                {
                    return protector.Unprotect(encryptedData);
                }
                catch (CryptographicException)
                {
                    return encryptedData;
                }
            }

            return encryptedData;
        }

        private static bool LooksLikeDataProtectionPayload(string encryptedData)
        {
            return !string.IsNullOrWhiteSpace(encryptedData)
                && encryptedData.Length >= DataProtectionPayloadPrefix.Length
                && encryptedData.StartsWith(DataProtectionPayloadPrefix, StringComparison.Ordinal);
        }

        private string SafeDecryptData(string encryptedData, IDataProtector protector)
        {
            if (string.IsNullOrEmpty(encryptedData))
                return encryptedData;

            try
            {
                return DecryptData(encryptedData, protector);
            }
            catch
            {
                return encryptedData;
            }
        }

        private string EncryptData(string plainText, IDataProtector protector)
        {
            if (LooksLikeDataProtectionPayload(plainText))
            {
                return plainText;
            }
            return string.IsNullOrEmpty(plainText)
                ? plainText
                : protector.Protect(plainText);
        }

        private bool Compare(string encryptedData, string inputData, IDataProtector protector)
        {
            try
            {
                return protector.Unprotect(encryptedData) == inputData;
            }
            catch
            {
                return false;
            }
        }

        public bool CompareCustomerData(string encryptedData, string inputData)
            => Compare(encryptedData, inputData, _customerProtector);

        public bool CompareEmployeeData(string encryptedData, string inputData)
            => Compare(encryptedData, inputData, _employeeProtector);

        public string SafeDecryptEmployeeData(string encryptedData)
            => SafeDecryptData(encryptedData, _employeeProtector);

        public string EncryptEmployeeData(string plainText)
            => EncryptData(plainText, _employeeProtector);

        public string SafeDecryptReserData(string encryptedData)
            => SafeDecryptData(encryptedData, _reservationProtector);

        public string EncryptReserData(string plainText)
            => EncryptData(plainText, _reservationProtector);

        public string SafeDecryptCustomerData(string encryptedData)
            => SafeDecryptData(encryptedData, _customerProtector);

        public string EncryptCustomerData(string plainText)
            => EncryptData(plainText, _customerProtector);

        public string SafeDecryptAdministratorData(string encryptedData)
            => SafeDecryptData(encryptedData, _adminProtector);

        public string EncryptAdministratorData(string plainText)
            => EncryptData(plainText, _adminProtector);

        public string SafeDecryptTwoFactorData(string encryptedData)
            => SafeDecryptData(encryptedData, _twoFactorProtector);

        public string EncryptTwoFactorData(string plainText)
            => EncryptData(plainText, _twoFactorProtector);

        public bool IsTwoFactorDataProtected(string encryptedData)
            => LooksLikeDataProtectionPayload(encryptedData);
    }
}
