using FurniZone.BLL.ModelVM.Auth;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            var result = await _authService.SignUpAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            var result = await _authService.SignInAsync(request);
            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _authService.LogoutAsync(userId.Value);
            return Ok(result);
        }

        // POST /api/auth/create-admin (Admin only)
        [HttpPost("create-admin")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> CreateAdmin([FromBody] SignUpRequest request)
        {
            var result = await _authService.CreateAdminAsync(request);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        private Guid? GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                return userId;
            return null;
        }
    }
}
