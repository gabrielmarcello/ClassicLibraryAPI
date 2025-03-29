using ClassicLibraryAPI.Helpers;
using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using ClassicLibraryAPI.Models;
using AutoMapper;
using Dapper;
using System.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Services;
using Microsoft.Data.SqlClient;

namespace ClassicLibraryAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase {

        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly IMapper _mapper;
        private readonly ICryptographyService _cryptographyService;

        public AuthController(IConfiguration config) {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _cryptographyService = new CryptographyService(config);
            _mapper = new Mapper(new MapperConfiguration(cfg => {
                cfg.CreateMap<UserForRegistrationDTO, User>();
            }));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDTO userForRegistration) {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm) {
                string sqlCheckUserExists = "SELECT Email FROM ClassicLibrarySchema.Auth WHERE Email = '" +
                    userForRegistration.Email + "'";

                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0) {

                    UserForLoginDTO userForSetPassword = new UserForLoginDTO() {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };

                    if (_authHelper.SetPassword(userForSetPassword)) {

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
                            return Ok();
                        }
                        throw new Exception("Fail to add user");
                    }
                    throw new Exception("Failed to register user");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Passwords do not match!");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin) {
            string sqlForHashAndSalt = "EXEC ClassicLibrarySchema.spLoginConfirmation_Get @Email = @EmailParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@EmailParam", userForLogin.Email, DbType.String);

            try {
                UserForLoginConfirmationDTO userConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDTO>(sqlForHashAndSalt, sqlParameters);

                byte[] passwordHash = _cryptographyService.GetPasswordHash(userForLogin.Password, userConfirmation.PasswordSalt);

                if (!passwordHash.SequenceEqual(userConfirmation.PasswordHash)) {
                    return Unauthorized("Incorrect Password!");
                }

                string userIdSql = @"SELECT UserId FROM ClassicLibrarySchema.Users WHERE Email = @EmailParameter";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@EmailParameter", userForLogin.Email, DbType.String);


                int userId = _dapper.LoadDataSingleWithParameters<int>(userIdSql, parameters);

                return Ok(new Dictionary<string, string>{
                    {"token", _authHelper.CreateToken(userId)}
                });

            }
            catch (InvalidOperationException ex) {
                return StatusCode(500, "Couldn't find your account");
            }

            catch (SqlException ex) {
                return StatusCode(500, "Internal error, please try again later");
            }

            catch (Exception ex) {
                return StatusCode(500, "An unexpected error occured, please try again later");
            }
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDTO userForSetPassword) {
            try {
                if (_authHelper.SetPassword(userForSetPassword)) {
                    return Ok();
                }
            }
            catch (Exception ex) {
                return StatusCode(500, "An unexpected error occured, please try again later");
            }
            return BadRequest();
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken() {
            try {
                string userId = User.FindFirst("userId")?.Value + "";

                string userIdSql = "SELECT UserId FROM ClassicLibrarySchema.Users WHERE UserId = " + userId;

                int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

                return Ok(new Dictionary<string, string> {
                    {"token", _authHelper.CreateToken(userIdFromDb) }
                });
            } 
            catch (SqlException ex) {
                return StatusCode(500, "Internal error, please try again later");
            } 
            catch (Exception ex) {
                return StatusCode(500, "An unexpected error occured, please try again later");
            }
        }
    }
}