using System.Net;
using EuroMotors.Application.IntegrationTests.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

[Collection(nameof(IntegrationTestCollection))]
public class ProductControllerTests
{
    private readonly IntegrationTestWebAppFactory _factory;

    public ProductControllerTests(IntegrationTestWebAppFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetProducts_Should_Return_Paginated_Products()
    {
        // Act
        HttpResponseMessage response = await _factory.CreateClient().GetAsync("/api/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        string responseContent = await response.Content.ReadAsStringAsync();
        responseContent.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_ReturnOk_WhenFetchingProducts()
    {
        // Act
        HttpResponseMessage response = await _factory.CreateClient()
            .GetAsync("/api/products?pageNumber=1&pageSize=5&sortOrder=price_desc");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        string responseContent = await response.Content.ReadAsStringAsync();
        responseContent.ShouldNotBeNullOrEmpty();
        responseContent.ShouldContain("pageIndex");
        responseContent.ShouldContain("pageSize");
        responseContent.ShouldContain("5");
    }
}
