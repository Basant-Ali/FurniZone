using FurniZone.BLL.ModelVM.Order;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FurniZone.PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _orderService.GetUserOrdersAsync(userId.Value);
            return Ok(result);
        }

        [HttpGet("all")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilterRequest request)
        {
            var result = await _orderService.GetAllOrdersAsync(request);
            return Ok(result);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetOrder(Guid orderId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var isAdmin = User.IsInRole(nameof(UserRole.Admin));
            var result = await _orderService.GetOrderAsync(userId.Value, orderId, isAdmin);
            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized();

            var result = await _orderService.CreateOrderAsync(userId.Value);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("{orderId:guid}/status")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var result = await _orderService.UpdateStatusAsync(orderId, request.Status);
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
