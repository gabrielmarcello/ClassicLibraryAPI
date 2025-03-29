using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Models;
using ClassicLibraryAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase {

        private readonly IBookService _bookService;

        public BookController(IConfiguration config) {
            _bookService = new BookService(config);
        }

        [HttpGet("Books/{bookId}")]
        public IEnumerable<Book> GetBooks(int bookId) {
            return _bookService.GetBooks(bookId);

        }

    }
}
