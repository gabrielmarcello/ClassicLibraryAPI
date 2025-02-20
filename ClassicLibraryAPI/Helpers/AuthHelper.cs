using AutoMapper;
using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Dtos;
using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace ClassicLibraryAPI.Helpers {
    public class AuthHelper {

        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;

        public AuthHelper(IConfiguration config) {
            _config = config;
            _dapper = new DataContextDapper(config);
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

        public bool SetPassword(UserForLoginDTO userForSetPassword) {

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

            string sqlAddAuth = @"EXEC ClassicLibrarySchema.spRegistration_Upsert
                @Email = @EmailParam, 
                @PasswordHash = @PasswordHashParam, 
                @PasswordSalt = @PasswordSaltParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@EmailParam", userForSetPassword.Email, DbType.String);
            sqlParameters.Add("@PasswordHashParam", passwordHash, DbType.Binary);
            sqlParameters.Add("@PasswordSaltParam", passwordSalt, DbType.Binary);


            return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
        }
    }
}
