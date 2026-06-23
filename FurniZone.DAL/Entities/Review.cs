namespace FurniZone.DAL.Entities
{
    public class Review : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        // Navigation properties
        public Product Product { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
