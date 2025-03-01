using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase {

        private readonly DataContextDapper _dapper;

        public ReviewController(IConfiguration config) {
            _dapper = new DataContextDapper(config);
        }

        [AllowAnonymous]
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

        [Authorize]
        [ HttpPut("UpsertReview")]
        public IActionResult UpsertReview(Review reviewToUpsert) {
            string sql = @"EXEC ClassicLibrarySchema.spReviewUpsert
                            @UserId = @UserIdParameter,
                            @LivroId = @LivroIdParameter,
                            @Descricao = @DescricaoParameter,
                            @Avaliacao = @AvaliacaoParameter,
                            @Titulo = @TituloParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@UserIdParameter", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@ResenhaIdParameter", reviewToUpsert.ResenhaId, DbType.Int32);
            sqlParameters.Add("@TituloParameter", reviewToUpsert.Titulo, DbType.String);
            sqlParameters.Add("@AvaliacaoParameter", reviewToUpsert.Avaliacao, DbType.String);
            sqlParameters.Add("@LivroIdParameter", reviewToUpsert.LivroId, DbType.Int32);
            sqlParameters.Add("@DescricaoParameter", reviewToUpsert.Descricao, DbType.String);

            if (reviewToUpsert.ResenhaId > 0) {
                sql += ", @ResenhaId = @ResenhaIdParameter";
                sqlParameters.Add("@ResenhaIdParameter", reviewToUpsert.ResenhaId, DbType.Int32);
            }

            if(reviewToUpsert.Avaliacao <0 || reviewToUpsert.Avaliacao > 5) {
                return BadRequest("The rate is invalid");
            }

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) {
                return Ok();
            }

            throw new Exception("Failed to Upsert User");

        }
    }
}
