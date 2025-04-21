using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EuroMotors.Infrastructure.Configurations;

internal sealed class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> builder)
    {
        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.CarBrandId)
            .IsRequired();

        builder.HasOne(cm => cm.CarBrand)
            .WithMany(cb => cb.Models)
            .HasForeignKey(cm => cm.CarBrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(cm => cm.ModelName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cm => cm.StartYear)
            .IsRequired();

        builder.Property(cm => cm.BodyType)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(cm => cm.EngineSpec, engineSpec =>
        {
            engineSpec.Property(es => es.VolumeLiters).IsRequired();
            engineSpec.Property(es => es.FuelType).HasConversion<string>().IsRequired();
        });

        builder.Property(c => c.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.GenerateSlug(value));

        builder.Property(cm => cm.Slug)
            .IsRequired();

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.HasMany(cm => cm.Products)
            .WithOne()
            .HasForeignKey(p => p.CarModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
