using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Ocelot.ManualTest
{
    public class UserCredential
    {
        public string Hash { get; private set;}
        public string Salt { get; private set;}

        public UserCredential(string salt, string hash) 
        {
            this.Salt = salt;
            this.Hash = hash;
        }
    }

    public static class Utilities
    {
        public static UserCredential GenerateUserCredentials(string password)
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return new UserCredential(Convert.ToBase64String(salt), hashed);
        }
    }
}
