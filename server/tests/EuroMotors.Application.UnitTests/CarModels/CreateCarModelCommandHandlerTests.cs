using EuroMotors.Application.CarModels.CreateCarModel;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.CarModels;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;

namespace EuroMotors.Application.UnitTests.CarModels;

public class CreateCarModelCommandHandlerTests
{
    private readonly ICarModelRepository _carModelRepository = Substitute.For<ICarModelRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    private readonly CreateCarModelCommandHandler _handler;

    public CreateCarModelCommandHandlerTests()
    {
        _handler = new CreateCarModelCommandHandler(
            _carModelRepository,
            _unitOfWork);
    }

    [Fact]
    public async Task Handle_ShouldCreateCarModelWithoutImage()
    {
        // Arrange
        var engineSpec = new EngineSpec(6, FuelType.Diesel);
        var command = new CreateCarModelCommand(
            "BMW",
            "X5",
            2022,
            BodyType.SUV,
            engineSpec,
            null);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _carModelRepository.Received(1).Insert(Arg.Is<CarModel>(c => 
            c.Brand == "BMW" && 
            c.Model == "X5" && 
            c.StartYear == 2022 && 
            c.BodyType == BodyType.SUV));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldCreateCarModelWithImage()
    {
        // Arrange
        var engineSpec = new EngineSpec(6, FuelType.Diesel);
        
        IFormFile? mockFile = Substitute.For<IFormFile>();
        var fileContent = new MemoryStream();
        mockFile.OpenReadStream().Returns(fileContent);
        mockFile.FileName.Returns("test.jpg");
        mockFile.Length.Returns(1024);

        var command = new CreateCarModelCommand(
            "BMW",
            "X5",
            2022,
            BodyType.SUV,
            engineSpec,
            mockFile);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _carModelRepository.Received(1).Insert(Arg.Is<CarModel>(c => 
            c.Brand == "BMW" && 
            c.Model == "X5" && 
            c.StartYear == 2022 && 
            c.BodyType == BodyType.SUV && 
            c.ImagePath != null));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldCreateCarModelWithCorrectEngineSpec()
    {
        // Arrange
        var engineSpec = new EngineSpec(8, FuelType.Petrol);
        var command = new CreateCarModelCommand(
            "Mercedes",
            "S-Class",
            2023,
            BodyType.Sedan,
            engineSpec,
            null);

        // Act
        Result<Guid> result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsSuccess.ShouldBeTrue();
        _carModelRepository.Received(1).Insert(Arg.Is<CarModel>(c =>
            c.Brand == "Mercedes" &&
            Math.Abs(c.EngineSpec.VolumeLiters - 8) < 0.001 && 
            c.EngineSpec.FuelType == FuelType.Petrol));
        await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    }
} 
