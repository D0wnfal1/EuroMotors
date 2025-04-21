using EuroMotors.Domain.Products;
using EuroMotors.Domain.UnitTests.Infrastructure;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Products;

public class SpecificationTests : BaseTest
{
    private const string SpecName = "Color";
    private const string SpecValue = "Red";

    [Fact]
    public void Constructor_Should_SetPropertyValues()
    {
        // Act
        var specification = new Specification(SpecName, SpecValue);

        // Assert
        specification.SpecificationName.ShouldBe(SpecName);
        specification.SpecificationValue.ShouldBe(SpecValue);
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenSpecificationsHaveSameNameAndValue()
    {
        // Arrange
        var specification1 = new Specification(SpecName, SpecValue);
        var specification2 = new Specification(SpecName, SpecValue);

        // Act & Assert
        specification1.Equals(specification2).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenSpecificationsHaveDifferentNames()
    {
        // Arrange
        var specification1 = new Specification(SpecName, SpecValue);
        var specification2 = new Specification("Size", SpecValue);

        // Act & Assert
        specification1.Equals(specification2).ShouldBeFalse();
        (specification1 == specification2).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenSpecificationsHaveDifferentValues()
    {
        // Arrange
        var specification1 = new Specification(SpecName, SpecValue);
        var specification2 = new Specification(SpecName, "Blue");

        // Act & Assert
        specification1.Equals(specification2).ShouldBeFalse();
        (specification1 == specification2).ShouldBeFalse();
    }
}
