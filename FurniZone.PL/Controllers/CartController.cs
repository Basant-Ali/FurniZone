using FurniZone.BLL.ModelVM.Cart;
using FurniZone.BLL.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _cartService.GetCartAsync(userId.Value);
            return Ok(result);
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _cartService.AddToCartAsync(userId.Value, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("items/{cartItemId:guid}")]
        public async Task<IActionResult> UpdateCartItem(Guid cartItemId, [FromBody] UpdateCartItemRequest request)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _cartService.UpdateCartItemAsync(userId.Value, cartItemId, request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("items/{cartItemId:guid}")]
        public async Task<IActionResult> RemoveFromCart(Guid cartItemId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _cartService.RemoveFromCartAsync(userId.Value, cartItemId);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _cartService.ClearCartAsync(userId.Value);
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
