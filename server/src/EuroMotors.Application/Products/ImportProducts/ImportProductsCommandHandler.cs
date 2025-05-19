using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using EuroMotors.Domain.Categories;
using EuroMotors.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace EuroMotors.Application.Products.ImportProducts;


public sealed class ImportProductsCommandHandler : ICommandHandler<ImportProductsCommand, ImportProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICarModelRepository _carModelRepository;
    private readonly ICacheService _cacheService;
    private readonly IUnitOfWork _unitOfWork;

    public ImportProductsCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ICarModelRepository carModelRepository,
        ICacheService cacheService,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _carModelRepository = carModelRepository;
        _cacheService = cacheService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ImportProductsResult>> Handle(ImportProductsCommand request, CancellationToken cancellationToken)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Encoding = Encoding.UTF8,
            Delimiter = ",",
            MissingFieldFound = null
        };

        using var reader = new StreamReader(request.FileStream);
        using var csv = new CsvReader(reader, config);

#pragma warning disable CA1849
        var records = csv.GetRecords<ImportProductRequest>().ToList();
#pragma warning restore CA1849
        var errors = new List<string>();
        int successfullyImported = 0;

        List<CarModel> allCarModels = await _carModelRepository.GetAll()
            .Include(cm => cm.CarBrand)
            .ToListAsync(cancellationToken);

        foreach (ImportProductRequest record in records)
        {
            try
            {
                Category? category = await _categoryRepository.GetAll()
#pragma warning disable CA1311
#pragma warning disable CA1862
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == record.CategoryName.ToLower(), cancellationToken);
#pragma warning restore CA1862
#pragma warning restore CA1311

                if (category == null)
                {
                    errors.Add($"Category '{record.CategoryName}' not found for product '{record.Name}'");
                    continue;
                }

                var carModelNames = record.CarModelNames.Split(',', StringSplitOptions.RemoveEmptyEntries)
#pragma warning disable CA1311
                    .Select(n => n.Trim().ToLower())
#pragma warning restore CA1311
                    .ToList();

                var carModels = allCarModels
                    .Where(cm => carModelNames.Any(name =>
#pragma warning disable CA1311
#pragma warning disable CA1862
                        cm.CarBrand != null && ((cm.CarBrand.Name + " " + cm.ModelName).ToLower() == name ||
#pragma warning restore CA1862
#pragma warning restore CA1311
#pragma warning disable CA1311
#pragma warning disable CA1862
                                                cm.ModelName.ToLower() == name)))
#pragma warning restore CA1862
#pragma warning restore CA1311
                    .ToList();

                if (carModels.Count != carModelNames.Count)
                {
                    var foundModelNames = carModels.Select(cm =>
                    {
                        if (cm.CarBrand != null)
                        {
                            return $"{cm.CarBrand.Name} {cm.ModelName}";
                        }
                        return cm.ModelName;
                    }).ToList();
#pragma warning disable CA1311
                    IEnumerable<string> missingModels = carModelNames.Except(foundModelNames.Select(n => n.ToLower()),
#pragma warning restore CA1311
                        StringComparer.OrdinalIgnoreCase);
                    errors.Add($"Car models not found for product '{record.Name}': {string.Join(", ", missingModels)}");
                    continue;
                }

                List<(string SpecificationName, string SpecificationValue)> specifications = record.Specifications?.Select(s => (s.SpecificationName, s.SpecificationValue)).ToList()
                                                                                             ?? new List<(string SpecificationName, string SpecificationValue)>();

                var product = Product.Create(
                    record.Name,
                    specifications,
                    record.VendorCode,
                    category.Id,
                    carModels,
                    record.Price,
                    record.Discount,
                    record.Stock
                );

                await _productRepository.AddAsync(product, cancellationToken);
                successfullyImported++;
            }
            catch (Exception ex)
            {
                errors.Add($"Error importing product '{record.Name}': {ex.Message}");
            }
        }


        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _cacheService.RemoveByPrefixAsync(CacheKeys.Products.GetAllPrefix(), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.Categories.GetAllPrefix(), cancellationToken);
        await _cacheService.RemoveByPrefixAsync(CacheKeys.CarModels.GetAllPrefix(), cancellationToken);

        return new ImportProductsResult(
            records.Count,
            successfullyImported,
            records.Count - successfullyImported,
            errors);
    }
}


