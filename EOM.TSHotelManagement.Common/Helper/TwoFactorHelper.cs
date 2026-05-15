using EOM.TSHotelManagement.Infrastructure;
using System.Security.Cryptography;
using System.Text;

namespace EOM.TSHotelManagement.Common
{
    /// <summary>
    /// TOTP（2FA）工具类
    /// </summary>
    public class TwoFactorHelper
    {
        private const string Base32Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        private const string RecoveryCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        private readonly TwoFactorConfigFactory _configFactory;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configFactory"></param>
        public TwoFactorHelper(TwoFactorConfigFactory configFactory)
        {
            _configFactory = configFactory;
        }

        /// <summary>
        /// 生成 Base32 格式的 2FA 密钥
        /// </summary>
        /// <returns></returns>
        public string GenerateSecretKey()
        {
            var config = GetConfig();
            var secretSize = config.SecretSize <= 0 ? 20 : config.SecretSize;
            var secretBytes = RandomNumberGenerator.GetBytes(secretSize);
            return Base32Encode(secretBytes);
        }

        /// <summary>
        /// 生成恢复备用码（仅明文返回一次）
        /// </summary>
        /// <returns></returns>
        public List<string> GenerateRecoveryCodes()
        {
            var config = GetConfig();
            var result = new List<string>(config.RecoveryCodeCount);

            for (var i = 0; i < config.RecoveryCodeCount; i++)
            {
                var chars = new char[config.RecoveryCodeLength];
                for (var j = 0; j < chars.Length; j++)
                {
                    chars[j] = RecoveryCodeAlphabet[RandomNumberGenerator.GetInt32(0, RecoveryCodeAlphabet.Length)];
                }

                var raw = new string(chars);
                result.Add(FormatRecoveryCode(raw, config.RecoveryCodeGroupSize));
            }

            return result;
        }

        /// <summary>
        /// 生成恢复备用码盐值
        /// </summary>
        /// <returns></returns>
        public string CreateRecoveryCodeSalt()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
        }

        /// <summary>
        /// 对恢复备用码进行哈希
        /// </summary>
        /// <param name="recoveryCode">备用码（可带分隔符）</param>
        /// <param name="salt">盐值</param>
        /// <returns></returns>
        public string HashRecoveryCode(string recoveryCode, string salt)
        {
            var normalized = NormalizeRecoveryCode(recoveryCode);
            if (string.IsNullOrWhiteSpace(normalized) || string.IsNullOrWhiteSpace(salt))
            {
                return string.Empty;
            }

            if (!TryGetSaltBytes(salt, out var saltBytes))
            {
                return string.Empty;
            }

            using var hmac = new HMACSHA256(saltBytes);
            var payload = Encoding.UTF8.GetBytes(normalized);
            return Convert.ToHexString(hmac.ComputeHash(payload));
        }

        /// <summary>
        /// 校验恢复备用码
        /// </summary>
        /// <param name="recoveryCode">备用码（可带分隔符）</param>
        /// <param name="salt">盐值</param>
        /// <param name="expectedHash">库内哈希</param>
        /// <returns></returns>
        public bool VerifyRecoveryCode(string recoveryCode, string salt, string expectedHash)
        {
            if (string.IsNullOrWhiteSpace(expectedHash))
            {
                return false;
            }

            var currentHash = HashRecoveryCode(recoveryCode, salt);
            if (!string.IsNullOrWhiteSpace(currentHash) && FixedTimeEquals(currentHash, expectedHash))
            {
                return true;
            }

            // Compatibility for historical records created with legacy SHA256(salt:code).
            var legacyHash = HashRecoveryCodeLegacy(recoveryCode, salt);
            return !string.IsNullOrWhiteSpace(legacyHash) && FixedTimeEquals(legacyHash, expectedHash);
        }

        /// <summary>
        /// 归一化恢复备用码（去空格和连接符）
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string NormalizeRecoveryCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return string.Empty;
            }

            return new string(code
                .Where(c => char.IsLetterOrDigit(c))
                .Select(char.ToUpperInvariant)
                .ToArray());
        }

        /// <summary>
        /// 生成符合 Google Authenticator 的 otpauth URI
        /// </summary>
        /// <param name="accountName">账号标识</param>
        /// <param name="secretKey">Base32 密钥</param>
        /// <returns></returns>
        public string BuildOtpAuthUri(string accountName, string secretKey)
        {
            var config = GetConfig();
            var issuer = config.Issuer ?? "TSHotel";
            var encodedIssuer = Uri.EscapeDataString(issuer);
            var encodedAccount = Uri.EscapeDataString(accountName ?? "user");

            return $"otpauth://totp/{encodedIssuer}:{encodedAccount}?secret={secretKey}&issuer={encodedIssuer}&digits={config.CodeDigits}&period={config.TimeStepSeconds}";
        }

        /// <summary>
        /// 校验 TOTP 验证码
        /// </summary>
        /// <param name="secretKey">Base32 密钥</param>
        /// <param name="code">验证码</param>
        /// <param name="utcNow">校验时间（UTC，空时取当前）</param>
        /// <returns></returns>
        public bool VerifyCode(string secretKey, string code, DateTime? utcNow = null)
        {
            return TryVerifyCode(secretKey, code, out _, utcNow);
        }

        /// <summary>
        /// 校验 TOTP 验证码，并返回命中的计数器（counter）
        /// </summary>
        /// <param name="secretKey">Base32 密钥</param>
        /// <param name="code">验证码</param>
        /// <param name="validatedCounter">命中的计数器</param>
        /// <param name="utcNow">校验时间（UTC，空时取当前）</param>
        /// <returns></returns>
        public bool TryVerifyCode(string secretKey, string code, out long validatedCounter, DateTime? utcNow = null)
        {
            validatedCounter = -1;

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(code))
                return false;

            var config = GetConfig();
            var normalizedCode = new string(code.Where(char.IsDigit).ToArray());
            if (normalizedCode.Length != config.CodeDigits)
                return false;

            var key = Base32Decode(secretKey);
            var unixTime = new DateTimeOffset(utcNow ?? DateTime.UtcNow).ToUnixTimeSeconds();
            var step = config.TimeStepSeconds <= 0 ? 30 : config.TimeStepSeconds;
            var counter = unixTime / step;
            var drift = config.AllowedDriftWindows < 0 ? 0 : config.AllowedDriftWindows;

            for (var i = -drift; i <= drift; i++)
            {
                var currentCounter = counter + i;
                if (currentCounter < 0)
                    continue;

                var expected = ComputeTotp(key, currentCounter, config.CodeDigits);
                if (FixedTimeEquals(expected, normalizedCode))
                {
                    validatedCounter = currentCounter;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取验证码位数
        /// </summary>
        /// <returns></returns>
        public int GetCodeDigits()
        {
            return GetConfig().CodeDigits;
        }

        /// <summary>
        /// 获取时间步长（秒）
        /// </summary>
        /// <returns></returns>
        public int GetTimeStepSeconds()
        {
            return GetConfig().TimeStepSeconds;
        }

        private TwoFactorConfig GetConfig()
        {
            var config = _configFactory.GetTwoFactorConfig();
            if (config.CodeDigits is < 6 or > 8)
            {
                config.CodeDigits = 6;
            }

            if (config.TimeStepSeconds <= 0)
            {
                config.TimeStepSeconds = 30;
            }

            if (config.SecretSize <= 0)
            {
                config.SecretSize = 20;
            }

            if (config.RecoveryCodeCount <= 0)
            {
                config.RecoveryCodeCount = 8;
            }

            if (config.RecoveryCodeLength < 8)
            {
                config.RecoveryCodeLength = 10;
            }

            if (config.RecoveryCodeGroupSize <= 0)
            {
                config.RecoveryCodeGroupSize = 5;
            }

            return config;
        }

        private static string FormatRecoveryCode(string raw, int groupSize)
        {
            if (string.IsNullOrWhiteSpace(raw) || groupSize <= 0)
            {
                return raw;
            }

            var normalized = raw.ToUpperInvariant();
            var sb = new StringBuilder(normalized.Length + normalized.Length / groupSize);

            for (var i = 0; i < normalized.Length; i++)
            {
                if (i > 0 && i % groupSize == 0)
                {
                    sb.Append('-');
                }

                sb.Append(normalized[i]);
            }

            return sb.ToString();
        }

        private static string ComputeTotp(byte[] key, long counter, int digits)
        {
            var counterBytes = BitConverter.GetBytes(counter);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(counterBytes);
            }

            using var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(counterBytes);
            var offset = hash[^1] & 0x0F;
            var binaryCode = ((hash[offset] & 0x7F) << 24)
                             | (hash[offset + 1] << 16)
                             | (hash[offset + 2] << 8)
                             | hash[offset + 3];

            var otp = binaryCode % (int)Math.Pow(10, digits);
            return otp.ToString().PadLeft(digits, '0');
        }

        private static bool FixedTimeEquals(string left, string right)
        {
            var leftBytes = Encoding.UTF8.GetBytes(left);
            var rightBytes = Encoding.UTF8.GetBytes(right);
            return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
        }

        private static bool TryGetSaltBytes(string salt, out byte[] saltBytes)
        {
            saltBytes = Array.Empty<byte>();
            if (string.IsNullOrWhiteSpace(salt))
            {
                return false;
            }

            try
            {
                saltBytes = Convert.FromHexString(salt.Trim());
                return saltBytes.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private string HashRecoveryCodeLegacy(string recoveryCode, string salt)
        {
            var normalized = NormalizeRecoveryCode(recoveryCode);
            if (string.IsNullOrWhiteSpace(normalized) || string.IsNullOrWhiteSpace(salt))
            {
                return string.Empty;
            }

            using var sha = SHA256.Create();
            var payload = Encoding.UTF8.GetBytes($"{salt}:{normalized}");
            return Convert.ToHexString(sha.ComputeHash(payload));
        }

        private static string Base32Encode(byte[] data)
        {
            if (data.Length == 0)
                return string.Empty;

            var output = new StringBuilder((int)Math.Ceiling(data.Length / 5d) * 8);
            var bitBuffer = 0;
            var bitCount = 0;

            foreach (var b in data)
            {
                bitBuffer = (bitBuffer << 8) | b;
                bitCount += 8;

                while (bitCount >= 5)
                {
                    var index = (bitBuffer >> (bitCount - 5)) & 0x1F;
                    output.Append(Base32Alphabet[index]);
                    bitCount -= 5;
                }
            }

            if (bitCount > 0)
            {
                var index = (bitBuffer << (5 - bitCount)) & 0x1F;
                output.Append(Base32Alphabet[index]);
            }

            return output.ToString();
        }

        private static byte[] Base32Decode(string base32)
        {
            var normalized = (base32 ?? string.Empty)
                .Trim()
                .TrimEnd('=')
                .Replace(" ", string.Empty)
                .ToUpperInvariant();

            if (normalized.Length == 0)
                return Array.Empty<byte>();

            var bytes = new List<byte>(normalized.Length * 5 / 8);
            var bitBuffer = 0;
            var bitCount = 0;

            foreach (var c in normalized)
            {
                var index = Base32Alphabet.IndexOf(c);
                if (index < 0)
                {
                    throw new ArgumentException("Invalid Base32 secret key.");
                }

                bitBuffer = (bitBuffer << 5) | index;
                bitCount += 5;

                if (bitCount >= 8)
                {
                    bytes.Add((byte)((bitBuffer >> (bitCount - 8)) & 0xFF));
                    bitCount -= 8;
                }
            }

            return bytes.ToArray();
        }
    }
}
