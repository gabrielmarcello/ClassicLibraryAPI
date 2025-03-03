using AutoMapper;
using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Dtos;
using ClassicLibraryAPI.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ClassicLibraryAPI.Helpers {
    public class AuthHelper {

        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;
        private readonly ICryptographyService _cryptographyService;

        public AuthHelper(IConfiguration config) {
            _config = config;
            _dapper = new DataContextDapper(config);
        }

        public bool SetPassword(UserForLoginDTO userForSetPassword) {

            byte[] passwordSalt = new byte[128 / 8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = _cryptographyService.GetPasswordHash(userForSetPassword.Password, passwordSalt);

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

        public string CreateToken(int userId) {
            Claim[] claims = new Claim[]{
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString != null ? tokenKeyString : ""
                    )
                );

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor() {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
