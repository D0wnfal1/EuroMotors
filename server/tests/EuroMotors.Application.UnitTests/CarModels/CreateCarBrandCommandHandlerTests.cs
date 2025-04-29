using EuroMotors.Application.Abstractions.Caching;
using EuroMotors.Application.CarBrands.CreateCarBrand;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarBrands;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class CreateCarBrandCommandHandlerTests
{
    private readonly ICarBrandRepository _carBrandRepository = Substitute.For<ICarBrandRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateCarBrandCommandHandler _handler;

    public CreateCarBrandCommandHandlerTests()
    {
        ICacheService? cacheService = Substitute.For<ICacheService>();
        _handler = new CreateCarBrandCommandHandler(
            _carBrandRepository,
            cacheService,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateCarBrandWithoutLogo()
    {
        // Arrange
        var command = new CreateCarBrandCommand("BMW", null);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _carBrandRepository.Received(1).Insert(Arg.Is<CarBrand>(c => c.Name == "BMW"));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldCreateCarBrandWithLogo()
    {
        // Arrange
        IFormFile? mockFile = Substitute.For<IFormFile>();
        var fileContent = new MemoryStream();
        mockFile.OpenReadStream().Returns(fileContent);
        mockFile.FileName.Returns("logo.png");
        mockFile.Length.Returns(1024);

        var command = new CreateCarBrandCommand("Mercedes", mockFile);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _carBrandRepository.Received(1).Insert(Arg.Is<CarBrand>(c =>
            c.Name == "Mercedes" &&
            c.LogoPath != null));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldGenerateCorrectSlug()
    {
        // Arrange
        var command = new CreateCarBrandCommand("BMW X Series", null);

        // We need to capture the created brand to check its slug
        _carBrandRepository.When(x => x.Insert(Arg.Any<CarBrand>()))
            .Do(info =>
            {
                CarBrand? brand = info.Arg<CarBrand>();
                brand.Slug.Value.ShouldBe("bmw-x-series");
            });

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _carBrandRepository.Received(1).Insert(Arg.Any<CarBrand>());
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
}
