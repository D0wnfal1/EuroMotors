using EuroMotors.Domain.CarModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EuroMotors.Infrastructure.Configurations;

internal sealed class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> builder)
    {
        builder.HasKey(cm => cm.Id);

        builder.Property(cm => cm.Brand)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(cm => cm.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(cm => cm.Products)
            .WithOne()
            .HasForeignKey(p => p.CarModelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
