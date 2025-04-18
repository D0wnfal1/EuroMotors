using System.Net;
using EuroMotors.Application.IntegrationTests.Abstractions;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Products;

public class ProductControllerTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public ProductControllerTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_Should_Return_Paginated_Products()
    {
        // Act
        HttpResponseMessage response = await _client.GetAsync("/api/products");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        string responseContent = await response.Content.ReadAsStringAsync();
        responseContent.ShouldNotBeNullOrEmpty();
    }

}
