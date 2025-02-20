﻿using ClassicLibraryAPI.Helpers;
using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using ClassicLibraryAPI.Models;
using AutoMapper;
using Dapper;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    [ApiController]
    public class AuthController : ControllerBase {

        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly IMapper _mapper;

        public AuthController(IConfiguration config) {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
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
    }
}
