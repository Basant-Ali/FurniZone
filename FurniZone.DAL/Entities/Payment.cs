using FurniZone.DAL.Enums;

namespace FurniZone.DAL.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
    }
}
