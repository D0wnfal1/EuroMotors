using EuroMotors.Api.Controllers.Delivery;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Delivery.GetWarehouse;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Delivery;

public class DeliveryControllerTests
{
    private readonly DeliveryController _controller;
    private readonly IQueryHandler<GetWarehousesQuery, WarehousesResponse<Warehouse>> _getWarehousesHandler;

    public DeliveryControllerTests()
    {
        _getWarehousesHandler = Substitute.For<IQueryHandler<GetWarehousesQuery, WarehousesResponse<Warehouse>>>();
        _controller = new DeliveryController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetWarehouses_ShouldReturnOk_WhenWarehousesFound()
    {
        // Arrange
        var request = new GetWareHousesRequest
        {
            City = "Kyiv",
            Query = "Center"
        };

        var warehouses = new List<Warehouse>
        {
            new Warehouse
            {
                SiteKey = "1",
                Description = "Central Warehouse",
                ShortAddress = "Main St, 123",
                Phone = "123456789",
                Number = "1",
                CityDescription = "Kyiv",
                RegionCity = "Kyiv",
                WarehouseStatus = "Active"
            }
        };

        var response = new WarehousesResponse<Warehouse>
        {
            Success = true,
            Data = warehouses,
            Errors = [],
            Warnings = []
        };

        _getWarehousesHandler.Handle(Arg.Any<GetWarehousesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        // Act
        IActionResult result = await _controller.GetWarehouses(request, _getWarehousesHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(response);

        await _getWarehousesHandler.Received(1).Handle(
            Arg.Is<GetWarehousesQuery>(query =>
                query.City == request.City &&
                query.Query == request.Query),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetWarehouses_ShouldReturnNotFound_WhenWarehousesNotFound()
    {
        // Arrange
        var request = new GetWareHousesRequest
        {
            City = "Unknown",
            Query = "Unknown"
        };

        _getWarehousesHandler.Handle(Arg.Any<GetWarehousesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<WarehousesResponse<Warehouse>>(Error.NotFound("Warehouses.NotFound", "Warehouses not found")));

        // Act
        IActionResult result = await _controller.GetWarehouses(request, _getWarehousesHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }
}
