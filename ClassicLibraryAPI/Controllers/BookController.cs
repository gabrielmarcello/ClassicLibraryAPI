using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class BookController {

        private readonly DataContextDapper _dapper;

        public BookController(IConfiguration config) {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("Books/{bookId}")]
        public IEnumerable<Book> GetBooks(int bookId) {
            string sql = "EXEC ClassicLibrarySchema.spBooks_Get";

            string stringParameters = "";

            DynamicParameters sqlParameters = new DynamicParameters();

            if (bookId > 0) {
                stringParameters += ", @LivroId = @LivroIdParameter";
                sqlParameters.Add("@LivroIdParameter", bookId, DbType.Int32);
            }

            if (stringParameters.Length > 0) {
                sql += stringParameters.Substring(1);
            }

            IEnumerable<Book> books = _dapper.LoadDataWithParameters<Book>(sql, sqlParameters);
            return books;

        }

    }
}
