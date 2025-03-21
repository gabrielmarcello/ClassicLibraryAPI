using ClassicLibraryAPI.Data;
using ClassicLibraryAPI.Interfaces;
using ClassicLibraryAPI.Models;
using ClassicLibraryAPI.Services;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Data;

namespace ClassicLibraryAPI.Controllers {
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReviewController : ControllerBase {

        private readonly IReviewService _reviewService;

        public ReviewController(IConfiguration config) {
            _reviewService = new ReviewService(config);
        }

        [AllowAnonymous]
        [HttpGet("Resenhas/{resenhaId}/{userId}")]
        public IEnumerable<Review> resenhas(int resenhaId, int userId) {
            try {
                return _reviewService.resenhas(resenhaId, userId);
            } catch (Exception ex) {
                return Enumerable.Empty<Review>();
            }
            
        }

        [HttpPut("UpsertReview")]
        public IActionResult UpsertReview(Review reviewToUpsert) {
           string? userId = this.User.FindFirst("userId")?.Value;
            try {
                if (_reviewService.UpsertReview(reviewToUpsert, userId)) {
                    return Ok();
                }
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }
           
           return BadRequest();
        }

        [HttpDelete("DeleteReview/{reviewId}")]
        public IActionResult DeleteReview (int reviewId) {
            string? userId = this.User.FindFirst("userId")?.Value;

            try {
                if (_reviewService.DeleteReview(reviewId, userId)) {
                    return Ok();
                }
            }
            catch (Exception ex) {
                return StatusCode(500, ex.Message);
            }

            return BadRequest();
        }
    }
}
