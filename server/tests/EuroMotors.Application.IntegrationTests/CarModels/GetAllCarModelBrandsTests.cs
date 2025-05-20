using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Application.CarModels.GetAllCarModelBrands;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.CarModels;

public class GetAllCarModelBrandsTests : BaseIntegrationTest
{
    public GetAllCarModelBrandsTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnEmptyList_WhenNoBrandsExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        var query = new GetAllCarModelBrandsQuery();

        // Act
        IQueryHandler<GetAllCarModelBrandsQuery, List<CarBrandResponse>> handler =
            ServiceProvider.GetRequiredService<IQueryHandler<GetAllCarModelBrandsQuery, List<CarBrandResponse>>>();
        Result<List<CarBrandResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(0);
    }

}
