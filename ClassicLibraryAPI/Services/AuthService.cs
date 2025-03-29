using AutoMapper;
using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Dtos;
using ClassicLibraryAPI.Helpers;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassicLibraryAPI.Services {
    public class AuthService : IAuthService {

        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly IMapper _mapper;
        private readonly ICryptographyService _cryptographyService;

        public AuthService(IConfiguration config) {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _cryptographyService = new CryptographyService(config);
            _mapper = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<UserForRegistrationDTO, User>();
            }));
        }

        public bool Register(UserForRegistrationDTO userForRegistration) {
            if (userForRegistration.Password != userForRegistration.PasswordConfirm) {
                throw new Exception("Passwords do not match!");
            }

            string sqlCheckUserExists = "SELECT Email FROM ClassicLibrarySchema.Auth WHERE Email = '" +
                userForRegistration.Email + "'";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
            if (existingUsers.Count() != 0) {
                throw new Exception("User with this email already exists!");
            }

            UserForLoginDTO userForSetPassword = new UserForLoginDTO() {
                Email = userForRegistration.Email,
                Password = userForRegistration.Password
            };

            if (!_authHelper.SetPassword(userForSetPassword)) {
                throw new Exception("Failed to register user");
            }

            User userComplete = _mapper.Map<User>(userForRegistration);

            string sql = @"EXEC ClassicLibrarySchema.spUser_Upsert
                            @FirstName= @FirstNameParameter, 
                            @LastName= @LastNameParameter,
                            @Email= @EmailParameter, 
                            @PhoneNumber= @PhoneNumberParameter, 
                            @Adress= @AdressParameter,
                            @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParameter", userComplete.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", userComplete.LastName, DbType.String);
            sqlParameters.Add("@EmailParameter", userComplete.Email, DbType.String);
            sqlParameters.Add("@PhoneNumberParameter", userComplete.PhoneNumber, DbType.String);
            sqlParameters.Add("@AdressParameter", userComplete.Adress, DbType.String);
            sqlParameters.Add("@UserIdParameter", userComplete.UserId, DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) {
                return true;
            }
            throw new Exception("Fail to add user");
        }

        public Dictionary<string, string> Login(UserForLoginDTO userForLogin) {
            string sqlForHashAndSalt = "EXEC ClassicLibrarySchema.spLoginConfirmation_Get @Email = @EmailParam";

            try {
                DynamicParameters sqlParameters = new DynamicParameters();

                sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);


                UserForLoginConfirmationDTO userConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDTO>(sqlForHashAndSalt, sqlParameters);

                byte[] passwordHash = _cryptographyService.GetPasswordHash(userForLogin.Password, userConfirmation.PasswordSalt);

                if (!passwordHash.SequenceEqual(userConfirmation.PasswordHash)) {
                    throw new Exception("Incorrect Password!");
                }

                string userIdSql = @"SELECT UserId FROM ClassicLibrarySchema.Users WHERE Email = @EmailParameter";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@EmailParameter", userForLogin.Email, DbType.String);


                int userId = _dapper.LoadDataSingleWithParameters<int>(userIdSql, parameters);

                return new Dictionary<string, string>{
                    {"token", _authHelper.CreateToken(userId)}
                };


            }
            catch (InvalidOperationException ex) {
                throw new Exception("Couldn't find your account", ex);
            }

            catch (SqlException ex) {
                throw new Exception("Internal error, please try again later", ex);
            }
        }
        public bool ResetPassword(UserForLoginDTO userForSetPassword) {
            try {
                if (_authHelper.SetPassword(userForSetPassword)) {
                    return true;
                }
            }
            catch (Exception ex) {
                throw new Exception("An unexpected error occured, please try again later", ex);
            }

            return false;
        }

        public Dictionary<string,string> RefreshToken(string userId) {
            try {
                string userIdSql = "SELECT UserId FROM ClassicLibrarySchema.Users WHERE UserId = " + userId;

                int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

                return new Dictionary<string, string> {
                    {"token", _authHelper.CreateToken(userIdFromDb) }
                };
            }
            catch (SqlException ex) {
                throw new Exception("Internal error, please try again later", ex);
            }
            catch (Exception ex) {
                throw new Exception("An unexpected error occured, please try again later", ex);
            }
        }

    }

}


