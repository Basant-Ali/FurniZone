using FurniZone.BLL.ModelVM.Common;
using FurniZone.BLL.ModelVM.Order;

namespace FurniZone.BLL.Services.Abstractions
{
    public interface IPaymentService
    {
        Task<ApiResponse<PaymentResponse>> ProcessPaymentAsync(Guid orderId);
    }
}
