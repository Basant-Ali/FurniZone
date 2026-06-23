namespace FurniZone.DAL.Entities
{
    public class Wishlist : BaseEntity
    {
        public Guid UserId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}
