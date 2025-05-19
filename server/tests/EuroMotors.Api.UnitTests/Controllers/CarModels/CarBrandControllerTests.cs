using EuroMotors.Api.Controllers.CarBrands;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarBrands.CreateCarBrand;
using EuroMotors.Application.CarBrands.DeleteCarBrand;
using EuroMotors.Application.CarBrands.GetCarBrandById;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Application.CarBrands.UpdateCarBrand;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.CarModels;

public sealed class CarBrandControllerTests
{
    private readonly CarBrandController _controller;
    private readonly IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>> _getCarBrandsHandler;
    private readonly IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse> _getCarBrandByIdHandler;
    private readonly ICommandHandler<CreateCarBrandCommand, Guid> _createCarBrandHandler;
    private readonly ICommandHandler<UpdateCarBrandCommand> _updateCarBrandHandler;
    private readonly ICommandHandler<DeleteCarBrandCommand> _deleteCarBrandHandler;

    public CarBrandControllerTests()
    {
        _getCarBrandsHandler = Substitute.For<IQueryHandler<GetCarBrandsQuery, Pagination<CarBrandResponse>>>();
        _getCarBrandByIdHandler = Substitute.For<IQueryHandler<GetCarBrandByIdQuery, CarBrandResponse>>();
        _createCarBrandHandler = Substitute.For<ICommandHandler<CreateCarBrandCommand, Guid>>();
        _updateCarBrandHandler = Substitute.For<ICommandHandler<UpdateCarBrandCommand>>();
        _deleteCarBrandHandler = Substitute.For<ICommandHandler<DeleteCarBrandCommand>>();

        _controller = new CarBrandController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetCarBrands_ShouldReturnOk_WhenBrandsFound()
    {
        // Arrange
        var carBrands = new List<CarBrandResponse>
        {
            new CarBrandResponse
            {
                Id = Guid.NewGuid(),
                Name = "BMW",
                Slug = "bmw",
                LogoPath = "/images/brands/bmw.jpg"
            }
        };

        var pagination = new Pagination<CarBrandResponse>
        {
            PageIndex = 1,
            PageSize = 10,
            Count = 1,
            Data = carBrands
        };

        _getCarBrandsHandler.Handle(Arg.Any<GetCarBrandsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        // Act
        IActionResult result = await _controller.GetCarBrands(_getCarBrandsHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(pagination);

        await _getCarBrandsHandler.Received(1).Handle(
            Arg.Is<GetCarBrandsQuery>(query =>
                query.PageNumber == 1 &&
                query.PageSize == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCarBrands_ShouldReturnNotFound_WhenBrandsNotFound()
    {
        // Arrange
        _getCarBrandsHandler.Handle(Arg.Any<GetCarBrandsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Pagination<CarBrandResponse>>(
                Error.NotFound("CarBrands.NotFound", "Car brands not found")));

        // Act
        IActionResult result = await _controller.GetCarBrands(_getCarBrandsHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCarBrandById_ShouldReturnOk_WhenBrandFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var carBrand = new CarBrandResponse
        {
            Id = id,
            Name = "BMW",
            Slug = "bmw",
            LogoPath = "/images/brands/bmw.jpg"
        };

        _getCarBrandByIdHandler.Handle(Arg.Any<GetCarBrandByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(carBrand));

        // Act
        IActionResult result = await _controller.GetCarBrandById(_getCarBrandByIdHandler, id, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(carBrand);

        await _getCarBrandByIdHandler.Received(1).Handle(
            Arg.Is<GetCarBrandByIdQuery>(query => query.CarBrandId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCarBrandById_ShouldReturnNotFound_WhenBrandNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _getCarBrandByIdHandler.Handle(Arg.Any<GetCarBrandByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<CarBrandResponse>(
                Error.NotFound("CarBrand.NotFound", "Car brand not found")));

        // Act
        IActionResult result = await _controller.GetCarBrandById(_getCarBrandByIdHandler, id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateCarBrand_ShouldReturnCreatedAtAction_WhenCreationSucceeds()
    {
        // Arrange
        var request = new CreateCarBrandRequest
        {
            Name = "BMW"
        };

        var brandId = Guid.NewGuid();

        _createCarBrandHandler.Handle(Arg.Any<CreateCarBrandCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(brandId));

        // Act
        IActionResult result = await _controller.CreateCarBrand(request, _createCarBrandHandler, CancellationToken.None);

        // Assert
        CreatedAtActionResult createdResult = result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(CarBrandController.GetCarBrandById));
        createdResult.RouteValues?["id"].ShouldBe(brandId);
        createdResult.Value.ShouldBe(brandId);

        await _createCarBrandHandler.Received(1).Handle(
            Arg.Is<CreateCarBrandCommand>(cmd =>
                cmd.Name == request.Name),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCarBrand_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var request = new CreateCarBrandRequest
        {
            Name = "BMW"
        };

        var error = Error.Failure("CarBrand.InvalidData", "Invalid car brand data");

        _createCarBrandHandler.Handle(Arg.Any<CreateCarBrandCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.CreateCarBrand(request, _createCarBrandHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateCarBrand_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCarBrandRequest
        {
            Name = "BMW Updated"
        };

        _updateCarBrandHandler.Handle(Arg.Any<UpdateCarBrandCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateCarBrand(id, request, _updateCarBrandHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _updateCarBrandHandler.Received(1).Handle(
            Arg.Is<UpdateCarBrandCommand>(cmd =>
                cmd.CarBrandId == id &&
                cmd.Name == request.Name),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCarBrand_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCarBrandRequest
        {
            Name = "BMW Updated"
        };

        var error = Error.NotFound("CarBrand.NotFound", "Car brand not found");

        _updateCarBrandHandler.Handle(Arg.Any<UpdateCarBrandCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateCarBrand(id, request, _updateCarBrandHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task DeleteCarBrand_ShouldReturnNoContent_WhenDeletionSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();

        _deleteCarBrandHandler.Handle(Arg.Any<DeleteCarBrandCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.DeleteCarBrand(id, _deleteCarBrandHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _deleteCarBrandHandler.Received(1).Handle(
            Arg.Is<DeleteCarBrandCommand>(cmd => cmd.CarBrandId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCarBrand_ShouldReturnBadRequest_WhenDeletionFails()
    {
        // Arrange
        var id = Guid.NewGuid();

        var error = Error.NotFound("CarBrand.NotFound", "Car brand not found");

        _deleteCarBrandHandler.Handle(Arg.Any<DeleteCarBrandCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.DeleteCarBrand(id, _deleteCarBrandHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
}
