using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EuroMotors.Infrastructure.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.OwnsMany(p => p.Specifications, b =>
        {
            b.WithOwner().HasForeignKey("ProductId");

            b.HasKey("ProductId", "SpecificationName");

            b.Property(s => s.SpecificationName)
                .IsRequired()
                .HasMaxLength(100);

            b.Property(s => s.SpecificationValue)
                .IsRequired()
                .HasMaxLength(500);

            b.ToTable("product_specifications");
        });

        builder.Property(p => p.VendorCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Discount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Stock)
            .IsRequired();

        builder.Property(p => p.IsAvailable)
            .IsRequired();

        builder.Property(p => p.SoldCount)
            .HasColumnName("sold_count")  
            .HasDefaultValue(0);

        builder.Property(c => c.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.GenerateSlug(value));

        builder.Property(cm => cm.Slug)
            .IsRequired();

        builder.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey("ProductId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.CarModels)
            .WithMany(cm => cm.Products)
            .UsingEntity(
                "ProductCarModel",
                l => l.HasOne(typeof(CarModel))
                    .WithMany()
                    .HasForeignKey("car_model_id")
                    .OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(typeof(Product))
                    .WithMany()
                    .HasForeignKey("product_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("product_id", "car_model_id");
                    j.ToTable("product_car_models");
                });
    }
}
