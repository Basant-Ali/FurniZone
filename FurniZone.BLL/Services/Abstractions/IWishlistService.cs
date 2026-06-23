using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Wishlist;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface IWishlistService
    {
        Task<ApiResponse<WishlistResponse>> GetWishlistAsync(Guid userId);
        Task<ApiResponse<WishlistResponse>> AddToWishlistAsync(Guid userId, AddToWishlistRequest request);
        Task<ApiResponse<WishlistResponse>> RemoveFromWishlistAsync(Guid userId, Guid wishlistItemId);
    }
}
