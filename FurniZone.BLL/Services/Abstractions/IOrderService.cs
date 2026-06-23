using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Order;
using FurniZone.DAL.Enums;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface IOrderService
    {
        Task<ApiResponse<OrderResponse>> CreateOrderAsync(Guid userId);
        Task<ApiResponse<OrderResponse>> GetOrderAsync(Guid userId, Guid orderId, bool isAdmin = false);
        Task<ApiResponse<List<OrderResponse>>> GetUserOrdersAsync(Guid userId);
        Task<ApiResponse<PagedResponse<OrderResponse>>> GetAllOrdersAsync(OrderFilterRequest request);
        Task<ApiResponse<OrderResponse>> UpdateStatusAsync(Guid orderId, OrderStatus status);
    }
}
