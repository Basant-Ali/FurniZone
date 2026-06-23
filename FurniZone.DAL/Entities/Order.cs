using FurniZone.DAL.Enums;

namespace FurniZone.DAL.Entities
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
    }
}
