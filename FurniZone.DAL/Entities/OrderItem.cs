namespace FurniZone.DAL.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigation properties
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
