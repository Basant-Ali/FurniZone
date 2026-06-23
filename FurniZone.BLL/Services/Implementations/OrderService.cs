using AutoMapper;
using FurniZone.BLL.Helpers;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Order;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Enums;
using FurniZone.DAL.Repositories.Abstractions;
using FurniZone.DAL.Repositories.Models;

namespace FurniZone.BLL.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPaginationHelper _paginationHelper;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPaginationHelper paginationHelper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationHelper = paginationHelper;
        }

        public async Task<ApiResponse<OrderResponse>> CreateOrderAsync(Guid userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return ApiResponse<OrderResponse>.ErrorResponse("Cart is empty");
            }

            // Validate stock and calculate total
            decimal totalPrice = 0;
            foreach (var cartItem in cart.CartItems)
            {
                if (cartItem.Product.Stock < cartItem.Quantity)
                {
                    return ApiResponse<OrderResponse>.ErrorResponse(
                        $"Insufficient stock for {cartItem.Product.Name}");
                }

                totalPrice += cartItem.Product.Price * cartItem.Quantity;
            }

            // Create order
            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Status = OrderStatus.Pending,
                TotalPrice = totalPrice,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Orders.AddAsync(order);

            // Create order items and update stock
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.Price,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.OrderItems.AddAsync(orderItem);

                // Update product stock
                var product = await _unitOfWork.Products.GetByIdAsync(cartItem.ProductId);
                if (product != null)
                {
                    product.Stock -= cartItem.Quantity;
                    _unitOfWork.Products.Update(product);
                }
            }

            // Clear cart
            _unitOfWork.CartItems.DeleteRange(cart.CartItems);

            await _unitOfWork.SaveChangesAsync();

            // Return order with details
            var orderWithDetails = await _unitOfWork.Orders.GetOrderWithDetailsAsync(order.Id);
            var response = _mapper.Map<OrderResponse>(orderWithDetails);
            return ApiResponse<OrderResponse>.SuccessResponse(response, "Order created successfully");
        }

        public async Task<ApiResponse<OrderResponse>> GetOrderAsync(Guid userId, Guid orderId, bool isAdmin = false)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
            if (order == null)
            {
                return ApiResponse<OrderResponse>.ErrorResponse("Order not found");
            }

            // Check ownership
            if (order.UserId != userId && !isAdmin)
            {
                return ApiResponse<OrderResponse>.ErrorResponse("Access denied");
            }

            var response = _mapper.Map<OrderResponse>(order);
            return ApiResponse<OrderResponse>.SuccessResponse(response);
        }

        public async Task<ApiResponse<List<OrderResponse>>> GetUserOrdersAsync(Guid userId)
        {
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            var response = _mapper.Map<List<OrderResponse>>(orders);
            return ApiResponse<List<OrderResponse>>.SuccessResponse(response);
        }

        public async Task<ApiResponse<PagedResponse<OrderResponse>>> GetAllOrdersAsync(OrderFilterRequest request)
        {
            var filterParams = new OrderFilterParams
            {
                Status = request.Status,
                UserId = request.UserId,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                SortBy = request.SortBy,
                SortDescending = request.SortDescending,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            var orders = await _unitOfWork.Orders.GetAllOrdersAsync(filterParams);
            var totalCount = await _unitOfWork.Orders.CountAllAsync(filterParams);

            var orderResponses = _mapper.Map<List<OrderResponse>>(orders);
            var pagedResponse = _paginationHelper.CreatePagedResponse(
                orderResponses, request.PageNumber, request.PageSize, totalCount);

            return ApiResponse<PagedResponse<OrderResponse>>.SuccessResponse(pagedResponse);
        }

        public async Task<ApiResponse<OrderResponse>> UpdateStatusAsync(Guid orderId, OrderStatus status)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
            if (order == null)
            {
                return ApiResponse<OrderResponse>.ErrorResponse("Order not found");
            }

            order.Status = status;
            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<OrderResponse>(order);
            return ApiResponse<OrderResponse>.SuccessResponse(response, "Order status updated");
        }
    }
}
