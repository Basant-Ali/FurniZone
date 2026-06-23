using FurniZone.BLL.ModelVM.Wishlist;
using FurniZone.BLL.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _wishlistService.GetWishlistAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _wishlistService.AddToWishlistAsync(userId.Value, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("items/{wishlistItemId:guid}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid wishlistItemId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _wishlistService.RemoveFromWishlistAsync(userId.Value, wishlistItemId);
            if (!result.Success)
                return NotFound(result);

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
