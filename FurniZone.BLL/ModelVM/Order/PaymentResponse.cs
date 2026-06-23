using FurniZone.DAL.Enums;

namespace FurniZone.BLL.ModelVM.Order
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
