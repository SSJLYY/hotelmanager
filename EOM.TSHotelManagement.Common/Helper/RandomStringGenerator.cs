using System.Security.Cryptography;

namespace EOM.TSHotelManagement.Common
{
    public class RandomStringGenerator
    {
        public string GenerateSecurePassword()
        {
            const string numbers = "0123456789";
            const string lowerLetters = "abcdefghijklmnopqrstuvwxyz";
            const string upperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var allChars = numbers + lowerLetters + upperLetters + specialChars;

            var chars = new char[12];

            chars[0] = GetRandomChar(numbers);
            chars[1] = GetRandomChar(lowerLetters);
            chars[2] = GetRandomChar(upperLetters);
            chars[3] = GetRandomChar(specialChars);

            for (int i = 4; i < 12; i++)
            {
                chars[i] = GetRandomChar(allChars);
            }

            for (int i = chars.Length - 1; i > 0; i--)
            {
                int swapIndex = GetRandomInt(i + 1);
                (chars[i], chars[swapIndex]) = (chars[swapIndex], chars[i]);
            }

            return new string(chars);
        }

        private static char GetRandomChar(string charSet)
        {
            var randomNumber = RandomNumberGenerator.GetInt32(charSet.Length);
            return charSet[randomNumber];
        }

        private static int GetRandomInt(int maxValue)
        {
            return RandomNumberGenerator.GetInt32(maxValue);
        }
    }
}
