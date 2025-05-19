using EuroMotors.Api.Controllers.CarModels;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.Abstractions.Pagination;
using EuroMotors.Application.CarBrands.GetCarBrands;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Application.CarModels.GetAllCarModelBrands;
using EuroMotors.Application.CarModels.GetCarModelById;
using EuroMotors.Application.CarModels.GetCarModels;
using EuroMotors.Application.CarModels.GetCarModelSelection;
using EuroMotors.Application.CarModels.UpdateCarModel;
using EuroMotors.Domain.CarModels;
using Microsoft.AspNetCore.Http;

namespace EuroMotors.Api.UnitTests.Controllers.CarModels;

public class CarModelControllerTests
{
    private readonly CarModelController _controller;
    private readonly IQueryHandler<GetCarModelsQuery, Pagination<CarModelResponse>> _getCarModelsHandler;
    private readonly IQueryHandler<GetCarModelByIdQuery, CarModelResponse> _getCarModelByIdHandler;
    private readonly IQueryHandler<GetAllCarModelBrandsQuery, List<CarBrandResponse>> _getAllBrandsHandler;
    private readonly IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse> _getCarSelectionHandler;
    private readonly ICommandHandler<CreateCarModelCommand, Guid> _createCarModelHandler;
    private readonly ICommandHandler<UpdateCarModelCommand> _updateCarModelHandler;
    private readonly ICommandHandler<DeleteCarModelCommand> _deleteCarModelHandler;

    public CarModelControllerTests()
    {
        _getCarModelsHandler = Substitute.For<IQueryHandler<GetCarModelsQuery, Pagination<CarModelResponse>>>();
        _getCarModelByIdHandler = Substitute.For<IQueryHandler<GetCarModelByIdQuery, CarModelResponse>>();
        _getAllBrandsHandler = Substitute.For<IQueryHandler<GetAllCarModelBrandsQuery, List<CarBrandResponse>>>();
        _getCarSelectionHandler = Substitute.For<IQueryHandler<GetCarModelSelectionQuery, CarModelSelectionResponse>>();
        _createCarModelHandler = Substitute.For<ICommandHandler<CreateCarModelCommand, Guid>>();
        _updateCarModelHandler = Substitute.For<ICommandHandler<UpdateCarModelCommand>>();
        _deleteCarModelHandler = Substitute.For<ICommandHandler<DeleteCarModelCommand>>();

        _controller = new CarModelController()
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task GetCarModels_ShouldReturnOk_WhenModelsFound()
    {
        // Arrange
        var carModels = new List<CarModelResponse>
        {
            new CarModelResponse
            {
                Id = Guid.NewGuid(),
                CarBrandId = Guid.NewGuid(),
                ModelName = "X5",
                StartYear = 2020,
                BodyType = "SUV"
            }
        };

        var pagination = new Pagination<CarModelResponse>
        {
            PageIndex = 1,
            PageSize = 10,
            Count = 1,
            Data = carModels
        };

        _getCarModelsHandler.Handle(Arg.Any<GetCarModelsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(pagination));

        // Act
        IActionResult result = await _controller.GetCarModels(_getCarModelsHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(pagination);

        await _getCarModelsHandler.Received(1).Handle(
            Arg.Is<GetCarModelsQuery>(query =>
                query.PageNumber == 1 &&
                query.PageSize == 10),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCarModels_ShouldReturnNotFound_WhenModelsNotFound()
    {
        // Arrange
        _getCarModelsHandler.Handle(Arg.Any<GetCarModelsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Pagination<CarModelResponse>>(Error.NotFound("CarModels.NotFound", "Car models not found")));

        // Act
        IActionResult result = await _controller.GetCarModels(_getCarModelsHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCarModelById_ShouldReturnOk_WhenModelFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        var carModel = new CarModelResponse
        {
            Id = id,
            ModelName = "X5",
            StartYear = 2020,
            BodyType = "SUV"
        };

        _getCarModelByIdHandler.Handle(Arg.Any<GetCarModelByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(carModel));

        // Act
        IActionResult result = await _controller.GetCarModelById(_getCarModelByIdHandler, id, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(carModel);

        await _getCarModelByIdHandler.Received(1).Handle(
            Arg.Is<GetCarModelByIdQuery>(query => query.CarModelId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCarModelById_ShouldReturnNotFound_WhenModelNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();

        _getCarModelByIdHandler.Handle(Arg.Any<GetCarModelByIdQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<CarModelResponse>(Error.NotFound("CarModel.NotFound", "Car model not found")));

        // Act
        IActionResult result = await _controller.GetCarModelById(_getCarModelByIdHandler, id, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAllBrands_ShouldReturnOk_WhenBrandsFound()
    {
        // Arrange
        var brands = new List<CarBrandResponse>
        {
            new CarBrandResponse { Id = Guid.NewGuid(), Name = "BMW", Slug = "bmw", LogoPath = null },
            new CarBrandResponse { Id = Guid.NewGuid(), Name = "Audi", Slug = "audi", LogoPath = null },
            new CarBrandResponse { Id = Guid.NewGuid(), Name = "Mercedes", Slug = "mercedes", LogoPath = null }
        };

        _getAllBrandsHandler.Handle(Arg.Any<GetAllCarModelBrandsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(brands));

        // Act
        IActionResult result = await _controller.GetAllBrands(_getAllBrandsHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(brands);

        await _getAllBrandsHandler.Received(1).Handle(
            Arg.Any<GetAllCarModelBrandsQuery>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetAllBrands_ShouldReturnNotFound_WhenBrandsNotFound()
    {
        // Arrange
        _getAllBrandsHandler.Handle(Arg.Any<GetAllCarModelBrandsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<List<CarBrandResponse>>(Error.NotFound("Brands.NotFound", "Brands not found")));

        // Act
        IActionResult result = await _controller.GetAllBrands(_getAllBrandsHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetCarSelection_ShouldReturnOk_WhenSelectionFound()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new SelectCarModelRequest
        {
            BrandId = brandId,
            Model = "X5",
            StartYear = 2020,
            BodyType = "SUV"
        };

        var response = new CarModelSelectionResponse
        {
            Ids = new List<Guid> { Guid.NewGuid() },
            Brands = new List<BrandInfo> { new BrandInfo { Id = Guid.NewGuid(), Name = "BMW" } },
            Models = new List<string> { "X5" },
            Years = new List<int> { 2020 },
            BodyTypes = new List<string> { "SUV" },
            EngineSpecs = new List<string> { "2.0L Diesel" }
        };

        _getCarSelectionHandler.Handle(Arg.Any<GetCarModelSelectionQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(response));

        // Act
        IActionResult result = await _controller.GetCarSelection(request, _getCarSelectionHandler, CancellationToken.None);

        // Assert
        OkObjectResult okResult = result.ShouldBeOfType<OkObjectResult>();
        okResult.Value.ShouldBe(response);

        await _getCarSelectionHandler.Received(1).Handle(
            Arg.Is<GetCarModelSelectionQuery>(query =>
                query.BrandId == request.BrandId &&
                query.ModelName == request.Model &&
                query.StartYear == request.StartYear &&
                query.BodyType == request.BodyType),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetCarSelection_ShouldReturnNotFound_WhenSelectionNotFound()
    {
        // Arrange
        var request = new SelectCarModelRequest
        {
            BrandId = Guid.NewGuid(),
            Model = "Unknown",
            StartYear = 9999,
            BodyType = "Unknown"
        };

        _getCarSelectionHandler.Handle(Arg.Any<GetCarModelSelectionQuery>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<CarModelSelectionResponse>(Error.NotFound("Selection.NotFound", "Car selection not found")));

        // Act
        IActionResult result = await _controller.GetCarSelection(request, _getCarSelectionHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task CreateCarModel_ShouldReturnCreatedAtAction_WhenCreationSucceeds()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new CreateCarModelRequest
        {
            CarBrandId = brandId,
            ModelName = "X5",
            StartYear = 2020,
            BodyType = BodyType.SUV,
            VolumeLiters = 2.0f,
            FuelType = FuelType.Diesel
        };

        var carModelId = Guid.NewGuid();

        _createCarModelHandler.Handle(Arg.Any<CreateCarModelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(carModelId));

        // Act
        IActionResult result = await _controller.CreateCarModel(request, _createCarModelHandler, CancellationToken.None);

        // Assert
        CreatedAtActionResult createdResult = result.ShouldBeOfType<CreatedAtActionResult>();
        createdResult.ActionName.ShouldBe(nameof(CarModelController.GetCarModelById));
        createdResult.RouteValues?["id"].ShouldBe(carModelId);
        createdResult.Value.ShouldBe(carModelId);

        await _createCarModelHandler.Received(1).Handle(
            Arg.Is<CreateCarModelCommand>(cmd =>
                cmd.CarBrandId == request.CarBrandId &&
                cmd.ModelName == request.ModelName &&
                cmd.StartYear == request.StartYear &&
                cmd.BodyType == request.BodyType),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateCarModel_ShouldReturnBadRequest_WhenCreationFails()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var request = new CreateCarModelRequest
        {
            CarBrandId = brandId,
            ModelName = "X5",
            StartYear = 2020,
            BodyType = BodyType.SUV,
            VolumeLiters = 2.0f,
            FuelType = FuelType.Diesel
        };

        var error = Error.Failure("CarModel.InvalidData", "Invalid car model data");

        _createCarModelHandler.Handle(Arg.Any<CreateCarModelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<Guid>(error));

        // Act
        IActionResult result = await _controller.CreateCarModel(request, _createCarModelHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task UpdateCarModel_ShouldReturnNoContent_WhenUpdateSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCarModelRequest
        {
            Model = "X5 Updated",
            StartYear = 2021,
            BodyType = BodyType.SUV,
            VolumeLiters = 3.0f,
            FuelType = FuelType.Diesel
        };

        _updateCarModelHandler.Handle(Arg.Any<UpdateCarModelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.UpdateCarModel(id, request, _updateCarModelHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _updateCarModelHandler.Received(1).Handle(
            Arg.Is<UpdateCarModelCommand>(cmd =>
                cmd.Id == id &&
                cmd.ModelName == request.Model &&
                cmd.StartYear == request.StartYear &&
                cmd.BodyType == request.BodyType &&
                Math.Abs(cmd.EngineVolumeLiters.GetValueOrDefault() - request.VolumeLiters.GetValueOrDefault()) < 0.0001f &&
                cmd.EngineFuelType == request.FuelType),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateCarModel_ShouldReturnBadRequest_WhenUpdateFails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateCarModelRequest
        {
            Model = "X5 Updated",
            StartYear = 2021,
            BodyType = BodyType.SUV,
            VolumeLiters = 3.0f,
            FuelType = FuelType.Diesel
        };

        var error = Error.NotFound("CarModel.NotFound", "Car model not found");

        _updateCarModelHandler.Handle(Arg.Any<UpdateCarModelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.UpdateCarModel(id, request, _updateCarModelHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }

    [Fact]
    public async Task DeleteCarModel_ShouldReturnNoContent_WhenDeletionSucceeds()
    {
        // Arrange
        var id = Guid.NewGuid();

        _deleteCarModelHandler.Handle(Arg.Any<DeleteCarModelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        IActionResult result = await _controller.DeleteCarModel(id, _deleteCarModelHandler, CancellationToken.None);

        // Assert
        result.ShouldBeOfType<NoContentResult>();

        await _deleteCarModelHandler.Received(1).Handle(
            Arg.Is<DeleteCarModelCommand>(cmd => cmd.CarModelId == id),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DeleteCarModel_ShouldReturnBadRequest_WhenDeletionFails()
    {
        // Arrange
        var id = Guid.NewGuid();

        var error = Error.NotFound("CarModel.NotFound", "Car model not found");

        _deleteCarModelHandler.Handle(Arg.Any<DeleteCarModelCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure(error));

        // Act
        IActionResult result = await _controller.DeleteCarModel(id, _deleteCarModelHandler, CancellationToken.None);

        // Assert
        BadRequestObjectResult badRequestResult = result.ShouldBeOfType<BadRequestObjectResult>();
        badRequestResult.Value.ShouldBe(error);
    }
}
