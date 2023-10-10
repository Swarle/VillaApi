using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;


namespace BusinessLogicLayer.Infrastructure
{
    public class PasswordHasher
    {
        private static int SaltSize = 128 / 8;
        private static int iterations = 10000;
        private static int SubKeyLenght = 258 / 8;
        private static RandomNumberGenerator _rng = new RNGCryptoServiceProvider();

        public static string HashPassword(string password)
        {
            byte[] salt = new byte[SaltSize];
            _rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1
                , iterations, SubKeyLenght);
            var hashed = new byte[1 + SaltSize + SubKeyLenght];
            hashed[0] = 0x00;
            Buffer.BlockCopy(salt, 0, hashed, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, hashed, 1 + SaltSize, SubKeyLenght);

            return Convert.ToHexString(hashed);

        }

        public static bool VerifyHashedPassword(string hashedPassword, string password)
        {

            var ByteHashedPassword = Convert.FromHexString(hashedPassword);


            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(ByteHashedPassword, 1, salt, 0, salt.Length);

            byte[] expectedSubKey = new byte[SubKeyLenght];
            Buffer.BlockCopy(ByteHashedPassword, 1 + salt.Length, expectedSubKey,
                0, expectedSubKey.Length);

            byte[] actualSubKey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA1,
                iterations, SubKeyLenght);

            return CryptographicOperations.FixedTimeEquals(actualSubKey, expectedSubKey);

        }
    }
}
