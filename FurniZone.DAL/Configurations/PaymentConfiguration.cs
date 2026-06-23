using FurniZone.DAL.Entities;
using FurniZone.DAL.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FurniZone.DAL.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.OrderId)
                .IsRequired();

            builder.HasIndex(p => p.OrderId)
                .IsUnique();

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
}
