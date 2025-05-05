using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.CarModels.DeleteCarModel;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using EuroMotors.Domain.CarModels;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class DeleteCarModelCommandHandlerTests
{
    private readonly ICarModelRepository _carModelRepository = Substitute.For<ICarModelRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCarModelCommandHandler _handler;
    private readonly Guid _brandId = Guid.NewGuid();
    private readonly CarBrand _carBrand;

    public DeleteCarModelCommandHandlerTests()
    {
        _carBrand = CarBrand.Create("BMW");

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(_carBrand, _brandId);

        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new DeleteCarModelCommandHandler(_carModelRepository, cacheService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCarModelNotFound()
    {
        // Arrange
        var command = new DeleteCarModelCommand(Guid.NewGuid());

        _carModelRepository.GetByIdAsync(command.CarModelId, CancellationToken.None)
            .Returns((CarModel)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarModel.NotFound");

        await _carModelRepository.DidNotReceive().Delete(Arg.Any<Guid>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldDeleteCarModel_WhenCarModelExists()
    {
        // Arrange
        var carModelId = Guid.NewGuid();
        var command = new DeleteCarModelCommand(carModelId);

        var carModel = CarModel.Create(
            _carBrand,
            "X5",
            2020,
            BodyType.SUV,
            new EngineSpec(2.0f, FuelType.Diesel));

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(carModel, carModelId);

        _carModelRepository.GetByIdAsync(carModelId, CancellationToken.None)
            .Returns(carModel);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _carModelRepository.Received(1).Delete(carModelId);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
