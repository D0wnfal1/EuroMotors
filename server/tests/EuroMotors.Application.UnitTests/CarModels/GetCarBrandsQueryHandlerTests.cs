using System.Data;
using Dapper;
using EuroMotors.Application.Abstractions.Data;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Domain.Abstractions;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class GetCarBrandsQueryHandlerTests
{
    private readonly IDbConnectionFactory _dbConnectionFactory = Substitute.For<IDbConnectionFactory>();
    private readonly IDbConnection _dbConnection = Substitute.For<IDbConnection>();
    private readonly GetCarBrandsQueryHandler _handler;

    public GetCarBrandsQueryHandlerTests()
    {
        _dbConnectionFactory.CreateConnection().Returns(_dbConnection);
        _handler = new GetCarBrandsQueryHandler(_dbConnectionFactory);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedBrands()
    {
        // Arrange
        var query = new GetCarBrandsQuery(1, 10);

        var brands = new List<CarBrandResponse>
        {
            new() { Id = Guid.NewGuid(), Name = "BMW", Slug = "bmw", LogoPath = "/images/brands/bmw.png" },
            new() { Id = Guid.NewGuid(), Name = "Audi", Slug = "audi", LogoPath = "/images/brands/audi.png" }
        };

        _dbConnection
            .QueryAsync<CarBrandResponse>(Arg.Any<string>(), Arg.Any<object>())
            .Returns(brands);

        _dbConnection
            .ExecuteScalarAsync<int>(Arg.Any<string>())
            .Returns(2);

        // Act
        Result<Pagination<CarBrandResponse>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.PageIndex.ShouldBe(1);
        result.Value.PageSize.ShouldBe(10);
        result.Value.Data.Count.ShouldBe(2);
        result.Value.Data.ElementAt(0).Name.ShouldBe("BMW");
        result.Value.Data.ElementAt(1).Name.ShouldBe("Audi");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoBrandsExist()
    {
        // Arrange
        var query = new GetCarBrandsQuery(1, 10);

        _dbConnection
            .QueryAsync<CarBrandResponse>(Arg.Any<string>(), Arg.Any<object>())
            .Returns(new List<CarBrandResponse>());

        _dbConnection
            .ExecuteScalarAsync<int>(Arg.Any<string>())
            .Returns(0);

        // Act
        Result<Pagination<CarBrandResponse>> result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
        result.Value.PageIndex.ShouldBe(1);
        result.Value.PageSize.ShouldBe(10);
        result.Value.Data.Count.ShouldBe(0);
    }
}
