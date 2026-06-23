using FurniZone.DAL.Enums;

namespace FurniZone.DAL.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.User;

        // Navigation properties
        public Cart? Cart { get; set; }
        public Wishlist? Wishlist { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
