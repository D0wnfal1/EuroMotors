using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.CarBrands.DeleteCarBrand;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class DeleteCarBrandCommandHandlerTests
{
    private readonly ICarBrandRepository _carBrandRepository = Substitute.For<ICarBrandRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCarBrandCommandHandler _handler;

    public DeleteCarBrandCommandHandlerTests()
    {
        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new DeleteCarBrandCommandHandler(_carBrandRepository, cacheService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCarBrandNotFound()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var command = new DeleteCarBrandCommand(brandId);

        _carBrandRepository.GetByIdAsync(brandId, CancellationToken.None)
            .Returns((CarBrand)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarBrand.NotFound");

        await _carBrandRepository.DidNotReceive().Delete(Arg.Any<Guid>());
        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldDeleteCarBrand_WhenCarBrandExists()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var command = new DeleteCarBrandCommand(brandId);

        var carBrand = CarBrand.Create("BMW");

        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(carBrand, brandId);

        _carBrandRepository.GetByIdAsync(brandId, CancellationToken.None)
            .Returns(carBrand);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _carBrandRepository.Received(1).Delete(brandId);
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
