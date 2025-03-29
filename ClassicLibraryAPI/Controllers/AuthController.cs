using ClassicLibraryAPI.Helpers;
using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using ClassicLibraryAPI.Models;
using AutoMapper;
using Dapper;
using System.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Services;
using Microsoft.Data.SqlClient;

namespace ClassicLibraryAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase {

        private readonly IAuthService _authService;

        public AuthController(IConfiguration config) {
            _authService = new AuthService(config);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDTO userForRegistration) {
            try {
                if (_authService.Register(userForRegistration)) {
                    return Ok();
                }
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }

            return NotFound();
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDTO userForLogin) {
            try {
                return Ok(_authService.Login(userForLogin));
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDTO userForSetPassword) {
            try {
                if (_authService.ResetPassword(userForSetPassword)) {
                    return Ok();
                }
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
            return NotFound();
        }

        [HttpGet("RefreshToken")]
        public IActionResult RefreshToken() {
            try {
                string userId = User.FindFirst("userId")?.Value + "";

                return Ok(_authService.RefreshToken(userId));
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
        }
    }
}