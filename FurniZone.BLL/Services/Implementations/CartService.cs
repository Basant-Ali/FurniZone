using AutoMapper;
using FurniZone.BLL.ModelVM.Cart;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.BLL.Services.Implementations
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse<CartResponse>> GetCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                // Create cart if doesn't exist
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,

                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
            }

            var response = _mapper.Map<CartResponse>(cart);
            return ApiResponse<CartResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<CartResponse>> AddToCartAsync(Guid userId, AddToCartRequest request)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Carts.AddAsync(cart);
            }

            // Check if product exists
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                return ApiResponse<CartResponse>.ErrorResponse("Product not found");
            }

            // Check stock
            if (product.Stock < request.Quantity)
            {
                return ApiResponse<CartResponse>.ErrorResponse("Insufficient stock");
            }

            // Check if item already in cart
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += request.Quantity;
                _unitOfWork.CartItems.Update(existingItem);
            }
            else
            {
                // Add new item
                var cartItem = new CartItem
                {
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();

            // Return updated cart
            var updatedCart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            var response = _mapper.Map<CartResponse>(updatedCart);
            return ApiResponse<CartResponse>.SuccessResponse(response, "Item added to cart");
        }

        public async Task<ApiResponse<CartResponse>> UpdateCartItemAsync(Guid userId, Guid cartItemId, UpdateCartItemRequest request)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                return ApiResponse<CartResponse>.ErrorResponse("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return ApiResponse<CartResponse>.ErrorResponse("Cart item not found");
            }

            if (request.Quantity <= 0)
            {
                // Remove item if quantity is 0 or less
                _unitOfWork.CartItems.Delete(cartItem);
            }
            else
            {
                // Check stock
                if (cartItem.Product.Stock < request.Quantity)
                {
                    return ApiResponse<CartResponse>.ErrorResponse("Insufficient stock");
                }

                cartItem.Quantity = request.Quantity;
                _unitOfWork.CartItems.Update(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();

            // Return updated cart
            var updatedCart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            var response = _mapper.Map<CartResponse>(updatedCart);
            return ApiResponse<CartResponse>.SuccessResponse(response, "Cart updated");
        }

        public async Task<ApiResponse<CartResponse>> RemoveFromCartAsync(Guid userId, Guid cartItemId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null)
            {
                return ApiResponse<CartResponse>.ErrorResponse("Cart not found");
            }

            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
            {
                return ApiResponse<CartResponse>.ErrorResponse("Cart item not found");
            }

            _unitOfWork.CartItems.Delete(cartItem);
            await _unitOfWork.SaveChangesAsync();

            // Return updated cart
            var updatedCart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            var response = _mapper.Map<CartResponse>(updatedCart);
            return ApiResponse<CartResponse>.SuccessResponse(response, "Item removed from cart");
        }

        public async Task<ApiResponse> ClearCartAsync(Guid userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return ApiResponse.SuccessResponse("Cart is already empty");
            }

            _unitOfWork.CartItems.DeleteRange(cart.CartItems);
            await _unitOfWork.SaveChangesAsync();

            return ApiResponse.SuccessResponse("Cart cleared");
        }
    }
}
