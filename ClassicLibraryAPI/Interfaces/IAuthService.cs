using ClassicLibraryAPI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace ClassicLibraryAPI.Interfaces {
    public interface IAuthService {

        public bool Register(UserForRegistrationDTO userForRegistration);
        public Dictionary<string, string> Login(UserForLoginDTO userForLogin);
        public bool ResetPassword(UserForLoginDTO userForSetPassword);
        public Dictionary<string, string> RefreshToken(string userId);

    }
}
