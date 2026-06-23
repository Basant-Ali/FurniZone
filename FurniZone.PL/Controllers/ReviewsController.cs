using FurniZone.BLL.ModelVM.Review;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(Guid productId)
        {
            var result = await _reviewService.GetProductReviewsAsync(productId);
            return Ok(result);
        }

        [HttpGet("my-reviews")]
        [Authorize]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _reviewService.GetUserReviewsAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateReviewRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _reviewService.CreateAsync(userId.Value, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{reviewId:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid reviewId, [FromBody] UpdateReviewRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _reviewService.UpdateAsync(userId.Value, reviewId, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{reviewId:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid reviewId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var isAdmin = User.IsInRole(nameof(UserRole.Admin));
            var result = await _reviewService.DeleteAsync(userId.Value, reviewId, isAdmin);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        private Guid? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;
            return null;
        }
    }
}
