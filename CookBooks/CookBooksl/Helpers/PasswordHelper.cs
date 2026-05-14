using System.Security.Cryptography;
using System.Text;

namespace CookBooks.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string password)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(saltBytes);

            string salt = Convert.ToBase64String(saltBytes);
            string hash = ComputeHash(password, salt);

            return $"{salt}:{hash}";
        }

        public static bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash) || !storedHash.Contains(':'))
                return false;

            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            string salt = parts[0];
            string expectedHash = parts[1];
            string actualHash = ComputeHash(password, salt);

            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(actualHash),
                Encoding.UTF8.GetBytes(expectedHash));
        }

        private static string ComputeHash(string password, string salt)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = SHA256.HashData(inputBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}