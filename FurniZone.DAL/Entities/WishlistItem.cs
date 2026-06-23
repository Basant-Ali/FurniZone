namespace FurniZone.DAL.Entities
{
    public class WishlistItem : BaseEntity
    {
        public Guid WishlistId { get; set; }
        public Guid ProductId { get; set; }

        // Navigation properties
        public Wishlist Wishlist { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
