using EuroMotors.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EuroMotors.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(u => u.City)
            .HasMaxLength(100)
            .IsRequired(false);
    }
}
