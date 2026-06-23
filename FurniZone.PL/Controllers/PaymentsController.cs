using FurniZone.BLL.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("order/{orderId:guid}")]
        public async Task<IActionResult> ProcessPayment(Guid orderId)
        {
            var result = await _paymentService.ProcessPaymentAsync(orderId);
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
