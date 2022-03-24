using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace General
{
    public static class Cryptography
    {
        public static byte[] CreatePasswordHash(string input, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        }

        public static bool CheckValidInput(string input, byte[] existHash, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != existHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static string GenerateSecret(int size = 16)
        {
            var randomBytes = new byte[size];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public static string CreateHash(this string value, string salt)
        {
            var valueBytes = KeyDerivation.Pbkdf2(value, Encoding.UTF8.GetBytes(salt),
                KeyDerivationPrf.HMACSHA512, 10000, 256 / 8);
            return Convert.ToBase64String(valueBytes);
        }

        public static bool ValidateHash(this string value, string salt, string hash)
        {
            return CreateHash(value, salt) == hash;
        }

        public static string HashLowerCase(string input)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public static string HashUpperCase(string input)
        {
            using var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder(hash.Length * 2);

            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        public static string SignBySha256(string message, string key)
        {
            var hmac = new HMACSHA256(GetByte(key));
            var hash = hmac.ComputeHash(GetByte(WebUtility.UrlDecode(message)));
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        private static byte[] GetByte(string text)
        {
            return new UTF8Encoding().GetBytes(text);
        }
    }
}
