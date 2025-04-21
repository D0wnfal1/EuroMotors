using EuroMotors.Api.Controllers.Delivery;
using EuroMotors.Application.Delivery.GetWarehouse;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.Delivery;

public class DeliveryControllerTests
{
    private readonly ISender _sender;
    private readonly DeliveryController _controller;

    public DeliveryControllerTests()
    {
        _sender = Substitute.For<ISender>();
        _controller = new DeliveryController(_sender)
        {
            // Set up HttpContext for the controller
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

        _sender.Send(Arg.Any<GetWarehousesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        // Act
        IActionResult result = await _controller.GetWarehouses(request);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(response);

        await _sender.Received(1).Send(
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

        _sender.Send(Arg.Any<GetWarehousesQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<WarehousesResponse<Warehouse>>(Error.NotFound("Warehouses.NotFound", "Warehouses not found")));

        // Act
        IActionResult result = await _controller.GetWarehouses(request);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }
}