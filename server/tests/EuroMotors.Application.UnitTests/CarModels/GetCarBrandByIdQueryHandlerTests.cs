using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.CarBrands.GetCarBrandById;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Domain.Abstractions;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class GetCarBrandByIdQueryHandlerTests
{
    private readonly IDbConnectionFactory _dbConnectionFactory = Substitute.For<IDbConnectionFactory>();
    private readonly IDbConnection _dbConnection = Substitute.For<IDbConnection>();
    private readonly GetCarBrandByIdQueryHandler _handler;
    private readonly Guid _brandId = Guid.NewGuid();

    public GetCarBrandByIdQueryHandlerTests()
    {
        _dbConnectionFactory.CreateConnection().Returns(_dbConnection);
        _handler = new GetCarBrandByIdQueryHandler(_dbConnectionFactory);
    }

    [Fact]
    public async Task Handle_ShouldReturnBrand_WhenBrandExists()
    {
        // Arrange
        var query = new GetCarBrandByIdQuery(_brandId);

        var expectedResponse = new CarBrandResponse
        {
            Id = _brandId,
            Name = "BMW",
            Slug = "bmw",
            LogoPath = "/images/brands/bmw.png"
        };

        _dbConnection
            .QuerySingleOrDefaultAsync<CarBrandResponse>(Arg.Any<string>(), Arg.Any<object>())
            .Returns(expectedResponse);

        // Act
        Result<CarBrandResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(_brandId);
        result.Value.Name.ShouldBe("BMW");
        result.Value.Slug.ShouldBe("bmw");
        result.Value.LogoPath.ShouldBe("/images/brands/bmw.png");
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenBrandDoesNotExist()
    {
        // Arrange
        var query = new GetCarBrandByIdQuery(_brandId);

        _dbConnection
            .QuerySingleOrDefaultAsync<CarBrandResponse>(Arg.Any<string>(), Arg.Any<object>())
            .Returns((CarBrandResponse)null);

        // Act
        Result<CarBrandResponse> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarBrand.NotFound");
        result.Error.Description.ShouldContain(_brandId.ToString());
    }
}
