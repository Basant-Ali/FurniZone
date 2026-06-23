using FurniZone.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniZone.DAL.Configurations
{
    public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
    {
        public void Configure(EntityTypeBuilder<Wishlist> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.UserId)
                .IsRequired();

            builder.HasIndex(w => w.UserId)
                .IsUnique();

            builder.Property(w => w.CreatedAt)
                .IsRequired();

            // One-to-Many: Wishlist -> WishlistItems
            builder.HasMany(w => w.WishlistItems)
                .WithOne(wi => wi.Wishlist)
                .HasForeignKey(wi => wi.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
