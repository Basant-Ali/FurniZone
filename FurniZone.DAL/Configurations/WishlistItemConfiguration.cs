using FurniZone.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniZone.DAL.Configurations
{
    public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
    {
        public void Configure(EntityTypeBuilder<WishlistItem> builder)
        {
            builder.HasKey(wi => wi.Id);

            builder.Property(wi => wi.CreatedAt)
                .IsRequired();

            // Many-to-One: WishlistItem -> Product
            builder.HasOne(wi => wi.Product)
                .WithMany(p => p.WishlistItems)
                .HasForeignKey(wi => wi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
