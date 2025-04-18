using EuroMotors.Domain.Abstractions;

namespace EuroMotors.Domain.Products;

public sealed class Specification : ValueObject
{
    public string SpecificationName { get; private set; } = null!;
    public string SpecificationValue { get; private set; } = null!;

    private Specification() { }

    public Specification(string specificationName, string specificationValue)
    {
        if (string.IsNullOrWhiteSpace(specificationName))
        {
            Result.Failure(ProductErrors.InvalidSpecificationName(specificationName));
        }

        if (string.IsNullOrWhiteSpace(specificationValue))
        {
            Result.Failure(ProductErrors.InvalidSpecificationValue(specificationValue));
        }

        SpecificationName = specificationName;
        SpecificationValue = specificationValue;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return SpecificationName;
        yield return SpecificationValue;
    }
}
