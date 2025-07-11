﻿using EuroMotors.Domain.Products;

namespace EuroMotors.Application.Products.GetProductById;

public sealed class ProductResponse
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; }
    public List<Guid> CarModelIds { get; set; } = [];
    public string Name { get; set; } = null!;
    public string VendorCode { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public string Slug { get; set; } = null!;
    public List<ProductImageResponse> Images { get; set; } = [];
    public List<Specification> Specifications { get; set; } = new List<Specification>();

    public ProductResponse() { }

}

