using FurniZone.BLL.ModelVM.Cart;
using FurniZone.BLL.ModelVM.Common;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface ICartService
    {
        Task<ApiResponse<CartResponse>> GetCartAsync(Guid userId);
        Task<ApiResponse<CartResponse>> AddToCartAsync(Guid userId, AddToCartRequest request);
        Task<ApiResponse<CartResponse>> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemRequest request);
        Task<ApiResponse<CartResponse>> RemoveFromCartAsync(Guid userId, Guid cartItemId);
        Task<ApiResponse> ClearCartAsync(Guid userId);
    }
}
