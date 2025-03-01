using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ReviewController {

        private readonly DataContextDapper _dapper;

        public ReviewController(IConfiguration config) {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Resenhas/{resenhaId}/{userId}")]
        public IEnumerable<Review> resenhas(int resenhaId, int userId) {
            string sql = "EXEC ClassicLibrarySchema.spResenhaGet";
            string parameters = "";

            DynamicParameters sqlParameters = new DynamicParameters();

            if (resenhaId > 0) {
                parameters += ", @ResenhaId = @ResenhaIdParameter";
                sqlParameters.Add("@ResenhaIdParameter", resenhaId, DbType.Int32);
            }

            if (userId > 0) {
                parameters += ", @UserId = @UserIdParameter";
                sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            }

            return _dapper.LoadDataWithParameters<Review>(sql, sqlParameters);
        }


    }
}
