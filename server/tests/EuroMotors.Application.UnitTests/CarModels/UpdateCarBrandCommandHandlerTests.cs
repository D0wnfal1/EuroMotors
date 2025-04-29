using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.CarBrands.UpdateCarBrand;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class UpdateCarBrandCommandHandlerTests
{
    private readonly ICarBrandRepository _carBrandRepository = Substitute.For<ICarBrandRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCarBrandCommandHandler _handler;

    public UpdateCarBrandCommandHandlerTests()
    {
        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new UpdateCarBrandCommandHandler(
            _carBrandRepository,
            cacheService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCarBrandNotFound()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var command = new UpdateCarBrandCommand(brandId, "Updated BMW", null);

        _carBrandRepository.GetByIdAsync(brandId, CancellationToken.None)
            .Returns((CarBrand)null);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("CarBrand.NotFound");

        await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateCarBrandNameOnly_WhenNoLogoProvided()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var carBrand = CarBrand.Create("BMW");
        var command = new UpdateCarBrandCommand(brandId, "Updated BMW", null);

        // Update the brand's id via reflection for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(carBrand, brandId);

        _carBrandRepository.GetByIdAsync(brandId, CancellationToken.None)
            .Returns(carBrand);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        carBrand.Name.ShouldBe("Updated BMW");
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldUpdateCarBrandWithLogo()
    {
        // Arrange
        var brandId = Guid.NewGuid();
        var carBrand = CarBrand.Create("BMW");

        IFormFile? mockFile = Substitute.For<IFormFile>();
        var fileContent = new MemoryStream();
        mockFile.OpenReadStream().Returns(fileContent);
        mockFile.FileName.Returns("logo.png");
        mockFile.Length.Returns(1024);

        var command = new UpdateCarBrandCommand(brandId, "Updated BMW", mockFile);

        // Update the brand's id via reflection for testing
        typeof(Entity)
            .GetProperty("Id")
            ?.SetValue(carBrand, brandId);

        _carBrandRepository.GetByIdAsync(brandId, CancellationToken.None)
            .Returns(carBrand);

        // Act
        Result result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        carBrand.Name.ShouldBe("Updated BMW");
        carBrand.LogoPath.ShouldNotBeNull();
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
