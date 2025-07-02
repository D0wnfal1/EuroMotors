using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace EuroMotors.Application.ProductImport.ImportExcelProducts;

public sealed class ImportExcelProductsCommandHandler : ICommandHandler<ImportExcelProductsCommand, ImportExcelProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICarBrandRepository _carBrandRepository;
    private readonly ICarModelRepository _carModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    private const string DefaultCategoryName = "Auto Parts";

    public ImportExcelProductsCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICarBrandRepository carBrandRepository,
        ICarModelRepository carModelRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _carBrandRepository = carBrandRepository;
        _carModelRepository = carModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportExcelProductsResult>> Handle(ImportExcelProductsCommand request, CancellationToken cancellationToken)
    {
        OfficeOpenXml.ExcelPackage.License.SetNonCommercialOrganization("EuroMotors");
        var errors = new List<string>();
        int successfullyImported = 0;
        int totalProcessed = 0;

        try
        {
            List<PriceFileRow> priceRows = ProcessPriceFile(request.PriceFileStream, errors);

            List<MappingFileRow> mappingRows = new();
            if (request.MappingFileStream != null)
            {
                mappingRows = ProcessMappingFile(request.MappingFileStream, errors);
            }

            List<MergedProductData> mergedData = MergeData(priceRows, mappingRows);
            totalProcessed = mergedData.Count;

            const int batchSize = 100;
            for (int i = 0; i < mergedData.Count; i += batchSize)
            {
                int batchCount = Math.Min(batchSize, mergedData.Count - i);
                List<MergedProductData> batch = mergedData.GetRange(i, batchCount);
                
                int batchSuccessCount = await ImportProducts(batch, errors, cancellationToken);
                
                try
                {
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    successfullyImported += batchSuccessCount;
                }
                catch (Exception ex)
                {
                    string errorMessage = ex.Message;
                    Exception? innerEx = ex.InnerException;
                    while (innerEx != null)
                    {
#pragma warning disable S1643
                        errorMessage += $" | Inner exception: {innerEx.Message}";
#pragma warning restore S1643
                        innerEx = innerEx.InnerException;
                    }
                    
                    errors.Add($"Error saving batch {i / batchSize + 1}: {errorMessage}");
                }
            }

            return Result.Success(new ImportExcelProductsResult(
                totalProcessed,
                successfullyImported,
                totalProcessed - successfullyImported,
                errors));
        }
        catch (Exception ex)
        {
            string errorMessage = ex.Message;
            Exception? innerEx = ex.InnerException;
            while (innerEx != null)
            {
#pragma warning disable S1643
                errorMessage += $" | Inner exception: {innerEx.Message}";
#pragma warning restore S1643
                innerEx = innerEx.InnerException;
            }
            
            errors.Add($"Error importing products: {errorMessage}");
            return Result.Success(new ImportExcelProductsResult(
                totalProcessed,
                successfullyImported,
                totalProcessed - successfullyImported,
                errors));
        }
    }

    private List<PriceFileRow> ProcessPriceFile(Stream fileStream, List<string> errors)
    {
        var priceRows = new List<PriceFileRow>();

        try
        {
            using var package = new ExcelPackage(fileStream);
            ExcelWorksheet? worksheet = package.Workbook.Worksheets[0]; 
            int rows = worksheet.Dimension.Rows;

            for (int row = 2; row <= rows; row++)
            {
                try
                {
                    string stockText = worksheet.Cells[row, 5].Text?.Trim() ?? string.Empty;
                    int stock = 0;

#pragma warning disable S6610
#pragma warning disable CA1866
#pragma warning disable CA1310
                    if (stockText.StartsWith(">"))
#pragma warning restore CA1310
#pragma warning restore CA1866
#pragma warning restore S6610
                    {
                        string numericPart = stockText.Substring(1);
                        if (int.TryParse(numericPart, out int parsedStock))
                        {
                            stock = parsedStock;
                        }
                    }
                    else if (int.TryParse(stockText, out int parsedStock))
                    {
                        stock = parsedStock;
                    }

                    var priceRow = new PriceFileRow
                    {
                        Code = worksheet.Cells[row, 1].Text?.Trim() ?? string.Empty,
                        Name = worksheet.Cells[row, 2].Text?.Trim() ?? string.Empty,
                        Brand = worksheet.Cells[row, 3].Text?.Trim() ?? string.Empty,
                        OE = worksheet.Cells[row, 4].Text?.Trim() ?? string.Empty,
                        Stock = stock,
                        PriceUAH = decimal.TryParse(worksheet.Cells[row, 6].Text.Replace(",", "."), 
                            System.Globalization.NumberStyles.Any, 
                            System.Globalization.CultureInfo.InvariantCulture, 
                            out decimal price) ? price : 0
                    };

                    if (string.IsNullOrEmpty(priceRow.Code) || string.IsNullOrEmpty(priceRow.Name))
                    {
                        continue;
                    }

                    priceRows.Add(priceRow);
                }
                catch (Exception ex)
                {
                    errors.Add($"Error processing row {row} in price file: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Error processing price file: {ex.Message}");
        }

        return priceRows;
    }

    private List<MappingFileRow> ProcessMappingFile(Stream fileStream, List<string> errors)
    {
        var mappingRows = new List<MappingFileRow>();

        try
        {
            using var package = new ExcelPackage(fileStream);
            ExcelWorksheet? worksheet = package.Workbook.Worksheets[0]; 
            int rows = worksheet.Dimension.Rows;

            for (int row = 2; row <= rows; row++)
            {
                try
                {
                    var mappingRow = new MappingFileRow
                    {
                        WarehouseNumber = worksheet.Cells[row, 1].Text?.Trim() ?? string.Empty,
                        Manufacturer = worksheet.Cells[row, 2].Text?.Trim() ?? string.Empty,
                        PartPurpose = worksheet.Cells[row, 3].Text?.Trim() ?? string.Empty,
                        CarMake = worksheet.Cells[row, 4].Text?.Trim() ?? string.Empty,
                        CarModel = worksheet.Cells[row, 5].Text?.Trim() ?? string.Empty
                    };

                    if (string.IsNullOrEmpty(mappingRow.WarehouseNumber))
                    {
                        continue;
                    }

                    mappingRows.Add(mappingRow);
                }
                catch (Exception ex)
                {
                    errors.Add($"Error processing row {row} in mapping file: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add($"Error processing mapping file: {ex.Message}");
        }

        return mappingRows;
    }

    private List<MergedProductData> MergeData(List<PriceFileRow> priceRows, List<MappingFileRow> mappingRows)
    {
        var result = new List<MergedProductData>();

        var mappingByWarehouseNumber = mappingRows
            .GroupBy(m => m.WarehouseNumber)
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (PriceFileRow priceRow in priceRows)
        {
            var merged = new MergedProductData
            {
                Code = priceRow.Code,
                Name = priceRow.Name,
                Brand = priceRow.Brand,
                OE = priceRow.OE,
                Stock = priceRow.Stock,
                PriceUAH = priceRow.PriceUAH
            };

            if (mappingByWarehouseNumber.TryGetValue(priceRow.Code, out List<MappingFileRow>? matchingMappings) && matchingMappings.Count > 0)
            {
                MappingFileRow match = matchingMappings[0];
                merged.PartPurpose = match.PartPurpose;
                merged.CarMake = match.CarMake;
                merged.CarModel = match.CarModel;
                merged.Manufacturer = match.Manufacturer;
            }

            result.Add(merged);
        }

        return result;
    }

    private async Task<int> ImportProducts(List<MergedProductData> mergedData, List<string> errors, CancellationToken cancellationToken)
    {
        int successCount = 0;

        var categoryCache = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);

        Category? defaultCategory = await _categoryRepository.GetAll()
            .FirstOrDefaultAsync(c => c.Name == DefaultCategoryName, cancellationToken);

        if (defaultCategory == null)
        {
            defaultCategory = Category.Create(DefaultCategoryName);
            _categoryRepository.Insert(defaultCategory);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        categoryCache[DefaultCategoryName] = defaultCategory;

        List<Category> existingCategories = await _categoryRepository.GetAll().ToListAsync(cancellationToken);
        foreach (Category cat in existingCategories)
        {
            if (!categoryCache.ContainsKey(cat.Name))
            {
                categoryCache[cat.Name] = cat;
            }
        }

        List<CarBrand> existingBrands = await _carBrandRepository.GetAll().ToListAsync(cancellationToken);
        var brandCache = new Dictionary<string, CarBrand>(StringComparer.OrdinalIgnoreCase);
        
        foreach (CarBrand brand in existingBrands)
        {
            brandCache[brand.Name] = brand;
        }

        List<CarModel> existingModels = await _carModelRepository.GetAll()
            .Include(cm => cm.CarBrand)
            .ToListAsync(cancellationToken);

        var modelCache = new Dictionary<string, CarModel>(StringComparer.OrdinalIgnoreCase);
        foreach (CarModel model in existingModels)
        {
            if (model.CarBrand != null)
            {
                string key = $"{model.CarBrand.Name}:{model.ModelName}";
                modelCache[key] = model;
            }
        }

        foreach (MergedProductData data in mergedData)
        {
            try
            {
                var carModels = new List<CarModel>();
                
                if (!string.IsNullOrEmpty(data.CarMake) && !string.IsNullOrEmpty(data.CarModel))
                {
                    CarBrand? carMakeBrand = null;
                    if (!string.IsNullOrWhiteSpace(data.CarMake))
                    {
#pragma warning disable S1066
                        if (!brandCache.TryGetValue(data.CarMake, out carMakeBrand))
#pragma warning restore S1066
                        {
                            carMakeBrand = CarBrand.Create(data.CarMake);
                            _carBrandRepository.Insert(carMakeBrand);
                            brandCache[data.CarMake] = carMakeBrand;
                        }
                    }

                    if (carMakeBrand != null)
                    {
                        string modelKey = $"{data.CarMake}:{data.CarModel}";
                        
                        if (!modelCache.TryGetValue(modelKey, out CarModel? carModel))
                        {
                            try
                            {
                                List<CarModel> modelsForCarMake = await _carModelRepository.GetAll()
                                    .Where(m => m.CarBrandId == carMakeBrand.Id && m.ModelName == data.CarModel)
                                    .ToListAsync(cancellationToken);

                                if (modelsForCarMake.Any())
                                {
#pragma warning disable S6608
                                    carModel = modelsForCarMake.First();
#pragma warning restore S6608
                                }
                                else
                                {
                                    carModel = CarModel.Create(
                                        carMakeBrand,
                                        data.CarModel,
                                        null,  
                                        null,  
                                        null   
                                    );

                                    _carModelRepository.Insert(carModel);
                                }
                                modelCache[modelKey] = carModel;
                            }
                            catch (Exception ex)
                            {
                                errors.Add($"Error creating car model {data.CarMake} {data.CarModel}: {ex.Message}");
                                continue;
                            }
                        }
                        
                        carModels.Add(carModel);
                    }
                }

                Category? category = defaultCategory;
                
                if (!string.IsNullOrWhiteSpace(data.PartPurpose))
                {
#pragma warning disable S1066
                    if (!string.IsNullOrWhiteSpace(data.PartPurpose) &&
#pragma warning restore S1066
                        !categoryCache.TryGetValue(data.PartPurpose, out category))
                    {
                        try
                        {
                            category = Category.Create(data.PartPurpose);
                            _categoryRepository.Insert(category);
                            categoryCache[data.PartPurpose] = category;
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Error creating category '{data.PartPurpose}': {ex.Message}");
                            category = defaultCategory; 
                        }
                    }
                }

                var specifications = new List<(string Name, string Value)>();
                
                if (!string.IsNullOrWhiteSpace(data.OE))
                {
                    string oeValue = data.OE.Length > 500 ? data.OE[..500] : data.OE;
                    specifications.Add(("OE", oeValue));
                }
                
                if (!string.IsNullOrWhiteSpace(data.Manufacturer))
                {
                    string manufacturerValue = data.Manufacturer.Length > 500 ? data.Manufacturer[..500] : data.Manufacturer;
                    specifications.Add(("Manufacturer", manufacturerValue));
                }
                
                if (!string.IsNullOrWhiteSpace(data.Brand) && 
                    (string.IsNullOrWhiteSpace(data.Manufacturer) || 
                     !data.Brand.Equals(data.Manufacturer, StringComparison.OrdinalIgnoreCase)))
                {
                    string brandValue = data.Brand.Length > 500 ? data.Brand[..500] : data.Brand;
                    specifications.Add(("Brand", brandValue));
                }
                
                try
                {
                    Product? existingProduct = await _productRepository.GetAll()
                        .FirstOrDefaultAsync(p => p.VendorCode == data.Code, cancellationToken);

                    if (existingProduct != null)
                    {
                        existingProduct.Update(
                            data.Name,
                            specifications,
                            data.Code,
                            category.Id,
                            carModels,
                            data.PriceUAH,
                            existingProduct.Discount,
                            data.Stock
                        );
                    }
                    else
                    {
                        var product = Product.Create(
                            data.Name,
                            specifications,
                            data.Code, 
                            category.Id,
                            carModels,
                            data.PriceUAH,
                            0, 
                            data.Stock
                        );

                        _productRepository.Insert(product);
                    }
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Error creating/updating product {data.Code}: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error importing product {data.Code} - {data.Name}: {ex.Message}");
            }
        }

        return successCount;
    }
} 
