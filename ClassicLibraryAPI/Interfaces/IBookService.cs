using ClassicLibraryAPI.Models;

namespace ClassicLibraryAPI.Interfaces {
    public interface IBookService {

        public IEnumerable<Book> GetBooks(int bookId);

    }
}
