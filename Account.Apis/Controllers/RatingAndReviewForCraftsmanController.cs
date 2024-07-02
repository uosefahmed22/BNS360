using Account.Core.Dtos.RatingAndReviewDto;
using Account.Core.IServices.Content;
using Account.Reposatory.Services.Content;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Account.Apis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingAndReviewForCraftsmanController : ControllerBase
    {
        private readonly IServiceForRatingAndReviewsForCraftsmen _serviceForRatingAndReviewsForCraftsmen;

        public RatingAndReviewForCraftsmanController(IServiceForRatingAndReviewsForCraftsmen serviceForRatingAndReviewsForCraftsmen)
        {
            _serviceForRatingAndReviewsForCraftsmen = serviceForRatingAndReviewsForCraftsmen;
        }
        [HttpPost]
        public async Task<IActionResult> AddRatingAndReview([FromBody] RatingAndReviewModelForCraftsmenDto model)
        {
            var response = await _serviceForRatingAndReviewsForCraftsmen.AddAsync(model);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{craftsmanId}")]
        public async Task<IActionResult> GetReviewsAndRatings(int craftsmanId)
        {
            var reviews = await _serviceForRatingAndReviewsForCraftsmen.GetReviewsAndRatings(craftsmanId);
            return Ok(reviews);
        }

        [HttpDelete("{userId}/{craftsmanId}/{reviewAndRatingId}")]
        public async Task<IActionResult> RemoveRatingAndReview(string userId, int craftsmanId, int reviewAndRatingId)
        {
            var response = await _serviceForRatingAndReviewsForCraftsmen.RemoveAsync(userId, craftsmanId, reviewAndRatingId);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("summary/{craftsmanId}")]
        public async Task<IActionResult> GetReviewsAndRatingsSummary(int craftsmanId)
        {
            var summary = await _serviceForRatingAndReviewsForCraftsmen.GetReviewsAndRatingsForCraftsmanAsync(craftsmanId);
            return Ok(summary);
        }

        [HttpDelete("review/{reviewAndRatingId}")]
        public async Task<IActionResult> RemoveReviewForAdminById(int reviewAndRatingId)
        {
            var response = await _serviceForRatingAndReviewsForCraftsmen.RemoveReviewForAdminAsync(reviewAndRatingId);
            return StatusCode(response.StatusCode, response);
        }
    }
}
