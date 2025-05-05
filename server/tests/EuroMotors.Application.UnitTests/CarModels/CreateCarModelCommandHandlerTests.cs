using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class CreateCarModelCommandHandlerTests
{
    private readonly ICarModelRepository _carModelRepository = Substitute.For<ICarModelRepository>();
    private readonly ICarBrandRepository _carBrandRepository = Substitute.For<ICarBrandRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateCarModelCommandHandler _handler;
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly CarBrand _carBrand;

    public CreateCarModelCommandHandlerTests()
    {
        _carBrand = CarBrand.Create("BMW");

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(_carBrand, _brandId);

        _carBrandRepository.GetByIdAsync(_brandId, Arg.Any<CancellationToken>())
            .Returns(_carBrand);

        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new CreateCarModelCommandHandler(
            _carModelRepository,
            _carBrandRepository,
            cacheService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenBrandNotFound()
    {
        // Arrange
        var invalidBrandId = Guid.NewGuid();
        var engineSpec = new EngineSpec(6, FuelType.Diesel);

        _carBrandRepository.GetByIdAsync(invalidBrandId, Arg.Any<CancellationToken>())
            .Returns((CarBrand)null);

        var command = new CreateCarModelCommand(
            invalidBrandId,
            "X5",
            2022,
            BodyType.SUV,
            engineSpec
            );

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarBrand.NotFound");

        _carModelRepository.DidNotReceive().Insert(Arg.Any<CarModel>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldCreateCarModelWithoutImage()
    {
        // Arrange
        var engineSpec = new EngineSpec(6, FuelType.Diesel);
        var command = new CreateCarModelCommand(
            _brandId,
            "X5",
            2022,
            BodyType.SUV,
            engineSpec
            );

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _carModelRepository.Received(1).Insert(Arg.Is<CarModel>(c =>
            c.CarBrandId == _brandId &&
            c.ModelName == "X5" &&
            c.StartYear == 2022 &&
            c.BodyType == BodyType.SUV));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldCreateCarModelWithCorrectEngineSpec()
    {
        // Arrange
        var engineSpec = new EngineSpec(8, FuelType.Petrol);
        var command = new CreateCarModelCommand(
            _brandId,
            "7 Series",
            2023,
            BodyType.Sedan,
            engineSpec
            );

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _carModelRepository.Received(1).Insert(Arg.Is<CarModel>(c =>
            c.CarBrandId == _brandId &&
            c.ModelName == "7 Series" &&
            Math.Abs(c.EngineSpec.VolumeLiters - 8) < 0.001 &&
            c.EngineSpec.FuelType == FuelType.Petrol));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
