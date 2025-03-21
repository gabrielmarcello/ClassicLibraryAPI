using ClassicLibraryAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace ClassicLibraryAPI.Interfaces {
    public interface IReviewService {

        public IEnumerable<Review> resenhas(int resenhaId, int userId);
        public bool UpsertReview(Review reviewToUpsert, string userId);
        public bool DeleteReview(int reviewId, string userId);

    }
}
