using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Models;
using ClassicLibraryAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase {

        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository) {
            _userRepository = userRepository;
        }

        [HttpGet("GetUsers/{userId}")]
        public IEnumerable<User> GetUsers(int userId) {
            return _userRepository.GetUsers(userId);
        }

        [HttpPut("UpsertUser")]
        public IActionResult UpsertUser(User user) {
            if (_userRepository.UpsertUser(user)) {
                return Ok();
            }
            else {
                return BadRequest();
            }
        }

        [HttpDelete("DeleteUser/{userId}")]
        public IActionResult DeleteUser(int userId) {
            if (_userRepository.DeleteUser(userId)) {
                return Ok();
            }
            else {
                return NotFound("User not found");
            }
        }
    }
}
