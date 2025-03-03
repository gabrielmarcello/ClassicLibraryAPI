using ClassicLibraryAPI.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace ClassicLibraryAPI.Services {
    public class CryptographyService : ICryptographyService {

        private readonly IConfiguration _config;

        public CryptographyService(IConfiguration config) {
            _config = config;
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt) {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
                Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8
            );
        }
    }
}
