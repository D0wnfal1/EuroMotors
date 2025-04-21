using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EuroMotors.Infrastructure.Configurations;

internal sealed class CarBrandConfiguration : IEntityTypeConfiguration<CarBrand>
{
    public void Configure(EntityTypeBuilder<CarBrand> builder)
    {
        builder.HasKey(cb => cb.Id);

        builder.Property(cb => cb.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cb => cb.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.GenerateSlug(value))
            .IsRequired();

        builder.HasIndex(cb => cb.Slug)
            .IsUnique();

        builder.Property(cb => cb.LogoPath);
    }
}
