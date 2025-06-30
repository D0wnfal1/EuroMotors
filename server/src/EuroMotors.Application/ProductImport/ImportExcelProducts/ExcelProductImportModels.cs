namespace EuroMotors.Application.ProductImport.ImportExcelProducts;
public sealed class PriceFileRow
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string OE { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal PriceUAH { get; set; }
}

public sealed class MappingFileRow
{
    public string WarehouseNumber { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string PartPurpose { get; set; } = string.Empty;
    public string CarMake { get; set; } = string.Empty;
    public string CarModel { get; set; } = string.Empty;
}

public sealed class MergedProductData
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string OE { get; set; } = string.Empty;
    public int Stock { get; set; }
    public decimal PriceUAH { get; set; }
    public string? PartPurpose { get; set; }
    public string? CarMake { get; set; }
    public string? CarModel { get; set; }
} 
