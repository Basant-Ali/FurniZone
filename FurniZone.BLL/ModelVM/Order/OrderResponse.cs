using FurniZone.BLL.ModelVM.User;
using FurniZone.DAL.Enums;

namespace FurniZone.BLL.ModelVM.Order
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public UserResponse? User { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemResponse> OrderItems { get; set; } = new();
        public PaymentResponse? Payment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
