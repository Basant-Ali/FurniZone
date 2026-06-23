using FurniZone.DAL.Entities;
using FurniZone.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniZone.DAL.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(o => o.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            // Many-to-One: Order -> User
            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Order -> OrderItems
            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One: Order -> Payment
            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
