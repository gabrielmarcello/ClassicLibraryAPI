using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    //[Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase {

        private readonly DataContextDapper _dapper;

        public UserController(IConfiguration config) {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TesteConnection() {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }

        [HttpGet("GetUsers/{userId}")]
        public IEnumerable<User> GetUsers(int userId) {
            string sql = @"EXEC ClassicLibrarySchema.spUsers_Get";
            string stringParameters = "";

            DynamicParameters sqlParameters = new DynamicParameters();

            if(userId != 0) {
                stringParameters += ", @UserId = @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }

            if(stringParameters.Length > 0) {
                sql += stringParameters.Substring(1);
            }
            IEnumerable<User> users = _dapper.LoadDataWithParameters<User>(sql, sqlParameters);
            return users;
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(User user) {
            string sql = @"EXEC ClassicLibrarySchema.spUser_Upsert
                @FirstName= @FirstNameParameter, 
                @LastName= @LastNameParameter,
                @Email= @EmailParameter, 
                @PhoneNumber= @PhoneNumberParameter, 
                @Adress= @AdressParameter,
                @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);
            sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
            sqlParameters.Add("@PhoneNumberParameter", user.PhoneNumber, DbType.String);
            sqlParameters.Add("@AdressParameter", user.Adress, DbType.String);
            sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);

            if(_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) {
                return Ok();
            }
            throw new Exception("Failed to Upsert User");
        }

        [HttpDelete]
        public IActionResult DeleteUser(int userId) {
            string sql = "EXEC ClassicLibrarySchema.spUser_Delete @UserId = " + userId.ToString();

            if (_dapper.ExecuteSql(sql)) {
                return Ok();
            }
            throw new Exception("Failed to delete user");
        }
    }
}
