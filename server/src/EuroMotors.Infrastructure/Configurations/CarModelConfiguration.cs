﻿using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Products;
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
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(cm => cm.ModelName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cm => cm.StartYear);

        builder.Property(cm => cm.BodyType)
            .HasConversion<string>();

        builder.OwnsOne(cm => cm.EngineSpec, engineSpec =>
        {
            engineSpec.Property(es => es.VolumeLiters);
            engineSpec.Property(es => es.FuelType).HasConversion<string>();
        });

        builder.Property(c => c.Slug)
            .HasConversion(
                slug => slug.Value,
                value => Slug.GenerateSlug(value));

        builder.Property(cm => cm.Slug)
            .IsRequired();

        builder.HasMany(cm => cm.Products)
            .WithMany(p => p.CarModels)
            .UsingEntity<Dictionary<string, object>>(
                "ProductCarModel",
                r => r.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("product_id")
                    .OnDelete(DeleteBehavior.Cascade),
                l => l.HasOne<CarModel>()
                    .WithMany()
                    .HasForeignKey("car_model_id")
                    .OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("product_id", "car_model_id");
                    j.ToTable("product_car_models");
                });
    }
}
