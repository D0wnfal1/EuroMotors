using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Orders;
using EuroMotors.Domain.Payments;
using EuroMotors.Domain.ProductImages;
using EuroMotors.Domain.Products;
using EuroMotors.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<Category> Categories { get; }
    DbSet<CarModel> CarModels { get; }
    DbSet<Order> Orders { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
