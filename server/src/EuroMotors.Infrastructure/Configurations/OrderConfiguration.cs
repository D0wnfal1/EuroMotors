using EuroMotors.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EuroMotors.Infrastructure.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(c => c.UserId)
            .IsRequired(false);

        builder.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(o => o.DeliveryMethod)
            .HasConversion<string>() 
            .IsRequired();

        builder.Property(o => o.ShippingAddress)
            .HasMaxLength(500) 
            .IsRequired(false);

        builder.Property(o => o.PaymentMethod)
            .HasConversion<string>()
            .IsRequired();
    }
}
