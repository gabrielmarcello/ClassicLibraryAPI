using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClassicLibraryAPI.Services {
    public class ReviewService : IReviewService {

        private readonly DataContextDapper _dapper;

        public ReviewService(IConfiguration config) {
            _dapper = new DataContextDapper(config);
        }

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

            if(parameters.Length > 0) {
                sql += parameters.Substring(1);
            }
            try {
                return _dapper.LoadDataWithParameters<Review>(sql, sqlParameters);
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public bool UpsertReview(Review reviewToUpsert, string userId) {
            string sql = @"EXEC ClassicLibrarySchema.spReviewUpsert
                            @UserId = @UserIdParameter,
                            @LivroId = @LivroIdParameter,
                            @Descricao = @DescricaoParameter,
                            @Avaliacao = @AvaliacaoParameter,
                            @Titulo = @TituloParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            sqlParameters.Add("@ResenhaIdParameter", reviewToUpsert.ResenhaId, DbType.Int32);
            sqlParameters.Add("@TituloParameter", reviewToUpsert.Titulo, DbType.String);
            sqlParameters.Add("@AvaliacaoParameter", reviewToUpsert.Avaliacao, DbType.String);
            sqlParameters.Add("@LivroIdParameter", reviewToUpsert.LivroId, DbType.Int32);
            sqlParameters.Add("@DescricaoParameter", reviewToUpsert.Descricao, DbType.String);

            if (reviewToUpsert.ResenhaId > 0) {
                sql += ", @ResenhaId = @ResenhaIdParameter";
                sqlParameters.Add("@ResenhaIdParameter", reviewToUpsert.ResenhaId, DbType.Int32);
            }

            if (reviewToUpsert.Avaliacao < 0 || reviewToUpsert.Avaliacao > 5) {
                throw new Exception("The rate is invalid");
            }

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) {
                return true;
            }

            throw new Exception("Failed to Upsert Review");
        }

        public bool DeleteReview(int reviewId, string userId) {
            string sql = @"EXEC ClassicLibrarySchema.spDeleteReview
                            @UserId = @UserIdParameter,
                            @ResenhaId = @ResenhaIdParameter";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
            sqlParameters.Add("@ResenhaIdParameter", reviewId, DbType.Int32);

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters)) {
                return true;
            }

            throw new Exception("Failed to Delete Review");
        }

    }
}
