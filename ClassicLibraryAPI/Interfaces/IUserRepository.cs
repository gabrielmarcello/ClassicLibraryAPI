using ClassicLibraryAPI.Models;

namespace ClassicLibraryAPI.Interfaces {
    public interface IUserRepository {

        public IEnumerable<User> GetUsers(int userId);

        public bool UpsertUser(User user);

        public bool DeleteUser(int userId);

    }
}
