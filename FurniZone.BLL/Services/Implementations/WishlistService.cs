using AutoMapper;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Wishlist;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.BLL.Services.Implementations
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WishlistService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<WishlistResponse>> GetWishlistAsync(Guid userId)
        {
            var wishlist = await _unitOfWork.Wishlists.GetWishlistWithItemsByUserIdAsync(userId);
            if (wishlist == null)
            {
                // Create wishlist if doesn't exist
                wishlist = new Wishlist
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Wishlists.AddAsync(wishlist);
                await _unitOfWork.SaveChangesAsync();
            }

            var response = _mapper.Map<WishlistResponse>(wishlist);
            return ApiResponse<WishlistResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<WishlistResponse>> AddToWishlistAsync(Guid userId, AddToWishlistRequest request)
        {
            var wishlist = await _unitOfWork.Wishlists.GetWishlistWithItemsByUserIdAsync(userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Wishlists.AddAsync(wishlist);
            }

            // Check if product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return ApiResponse<WishlistResponse>.ErrorResponse("Product not found");
            }

            // Check if already in wishlist
            if (wishlist.WishlistItems.Any(wi => wi.ProductId == request.ProductId))
            {
                return ApiResponse<WishlistResponse>.ErrorResponse("Product already in wishlist");
            }

            var wishlistItem = new WishlistItem
            {
                Id = Guid.NewGuid(),
                WishlistId = wishlist.Id,
                ProductId = request.ProductId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.WishlistItems.AddAsync(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            // Return updated wishlist
            var updatedWishlist = await _unitOfWork.Wishlists.GetWishlistWithItemsByUserIdAsync(userId);
            var response = _mapper.Map<WishlistResponse>(updatedWishlist);
            return ApiResponse<WishlistResponse>.SuccessResponse(response, "Item added to wishlist");
        }

        public async Task<ApiResponse<WishlistResponse>> RemoveFromWishlistAsync(Guid userId, Guid wishlistItemId)
        {
            var wishlist = await _unitOfWork.Wishlists.GetWishlistWithItemsByUserIdAsync(userId);
            if (wishlist == null)
            {
                return ApiResponse<WishlistResponse>.ErrorResponse("Wishlist not found");
            }

            var wishlistItem = wishlist.WishlistItems.FirstOrDefault(wi => wi.Id == wishlistItemId);
            if (wishlistItem == null)
            {
                return ApiResponse<WishlistResponse>.ErrorResponse("Wishlist item not found");
            }

            _unitOfWork.WishlistItems.Delete(wishlistItem);
            await _unitOfWork.SaveChangesAsync();

            // Return updated wishlist
            var updatedWishlist = await _unitOfWork.Wishlists.GetWishlistWithItemsByUserIdAsync(userId);
            var response = _mapper.Map<WishlistResponse>(updatedWishlist);
            return ApiResponse<WishlistResponse>.SuccessResponse(response, "Item removed from wishlist");
        }
    }
}
