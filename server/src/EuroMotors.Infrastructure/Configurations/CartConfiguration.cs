using EuroMotors.Domain.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EuroMotors.Infrastructure.Configurations;

internal sealed class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired(false); 

        builder.Property(c => c.SessionId)
            .IsRequired(false); 

        builder.HasIndex(c => new { c.UserId, c.SessionId })
            .IsUnique()
            .HasFilter("\"user_id\" IS NOT NULL OR \"session_id\" IS NOT NULL");

        builder.Ignore(c => c.TotalPrice);

        builder.HasMany(c => c.CartItems)
            .WithOne()
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
