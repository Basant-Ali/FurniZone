using AutoMapper;
using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Order;
using FurniZone.BLL.Services.Abstractions;
using FurniZone.DAL.Entities;
using FurniZone.DAL.Enums;
using FurniZone.DAL.Repositories.Abstractions;

namespace FurniZone.BLL.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Random _random;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _random = new Random();
        }

        public async Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(Guid orderId)
        {
            var order = await _unitOfWork.Orders.GetOrderWithDetailsAsync(orderId);
            if (order == null)
            {
                return ApiResponse<PaymentResponse>.ErrorResponse("Order not found");
            }

            // Check if payment already exists
            var existingPayment = await _unitOfWork.Payments.GetPaymentByOrderIdAsync(orderId);
            if (existingPayment != null)
            {
                var existingResponse = _mapper.Map<PaymentResponse>(existingPayment);
                return ApiResponse<PaymentResponse>.SuccessResponse(existingResponse, "Payment already processed");
            }

            // Simulate payment processing (random success/failure)
            var isSuccess = _random.Next(1, 11) <= 8; // 80% success rate

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Amount = order.TotalPrice,
                Status = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // Update order status based on payment result
            if (isSuccess)
            {
                order.Status = OrderStatus.Paid;
            }
            else
            {
                order.Status = OrderStatus.Pending; // Keep as pending to retry
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            var response = _mapper.Map<PaymentResponse>(payment);
            var message = isSuccess ? "Payment successful" : "Payment failed";

            return ApiResponse<PaymentResponse>.SuccessResponse(response, message);
        }
    }
}
