using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Models;
using Dapper;
using System.Data;

namespace ClassicLibraryAPI.Services {
    public class BookService : IBookService {

        private readonly DataContextDapper _dapper;

        public BookService(IConfiguration config) {
            _dapper = new DataContextDapper(config);
        }

        public IEnumerable<Book> GetBooks(int bookId = 0) {
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
