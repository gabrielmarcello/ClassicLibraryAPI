using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClassicLibraryAPI.Services {
    public class UserRepository : IUserRepository {

        private readonly DataContextDapper _dapper;

        public UserRepository(IConfiguration config) {
            _dapper = new DataContextDapper(config);
        }

        public IEnumerable<User> GetUsers(int userId) {
            string sql = @"EXEC ClassicLibrarySchema.spUsers_Get";
            string stringParameters = "";

            DynamicParameters sqlParameters = new DynamicParameters();

            if (userId != 0) {
                stringParameters += ", @UserId = @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }

            if (stringParameters.Length > 0) {
                sql += stringParameters.Substring(1);
            }

            try {
                IEnumerable<User> users = _dapper.LoadDataWithParameters<User>(sql, sqlParameters);
                return users;
            }
            catch (Exception ex) {
                throw new Exception("Failed to get users");
            }
        }
        public bool UpsertUser(User user) {
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

            try {
                return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
            }
            catch (Exception ex) {
                throw new Exception("Failed to upsert user");
            }
        }

        public bool DeleteUser(int userId) {
            string sql = "EXEC ClassicLibrarySchema.spUser_Delete @UserId = @UserIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);

            try {
                return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
            }

            catch (Exception ex) {
                throw new Exception("Failed to delete user");
            }
        }

    }
}
