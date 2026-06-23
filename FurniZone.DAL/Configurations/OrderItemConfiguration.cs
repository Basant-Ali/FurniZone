using FurniZone.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniZone.DAL.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(oi => oi.CreatedAt)
                .IsRequired();

            // Many-to-One: OrderItem -> Product
            builder.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
