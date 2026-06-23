using FurniZone.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FurniZone.DAL.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            
            // Seed Data - Initial admin user and sample categories
            SeedData(modelBuilder);
            
            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Default Admin User (Password: Admin123!)
            // PasswordHash generated with BCrypt for "Admin123!"
            var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = adminId,
                UserName = "admin",
                Email = "admin@furnizone.com",
                PasswordHash = "$2a$11$HQJECJYJGP.R.fOBlKkqV.ytAHfi0XDYN/2bNqA7N2h8dZEO7b6oO", // Admin123!
                Role = Enums.UserRole.Admin,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Create cart and wishlist for admin
            modelBuilder.Entity<Cart>().HasData(new Cart
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                UserId = adminId,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            modelBuilder.Entity<Wishlist>().HasData(new Wishlist
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                UserId = adminId,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });

            // Sample Categories
            var furnitureId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var electronicsId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var decorId = Guid.Parse("66666666-6666-6666-6666-666666666666");

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = furnitureId, Name = "Furniture", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = electronicsId, Name = "Electronics", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Category { Id = decorId, Name = "Home Decor", CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );
        }
    }
}
