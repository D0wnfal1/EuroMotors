using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Users.Register;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Users;

public class RegisterUserTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();

    public RegisterUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_RegisterUser_Successfully()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        var command = new RegisterUserCommand(
            _faker.Internet.Email(),
            _faker.Person.FirstName,
            _faker.Person.LastName,
            "Password123!");

        // Act
        ICommandHandler<RegisterUserCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand, Guid>>();
        Result<Guid> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(Guid.Empty);
        
        User? user = await DbContext.Users.FindAsync(result.Value);
        user.ShouldNotBeNull();
        user.Email.ShouldBe(command.Email);
        user.FirstName.ShouldBe(command.FirstName);
        user.LastName.ShouldBe(command.LastName);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenEmailAlreadyExists()
    {
        // Arrange
        await CleanDatabaseAsync();
        
        string email = _faker.Internet.Email();
        
        var command1 = new RegisterUserCommand(
            email,
            _faker.Person.FirstName,
            _faker.Person.LastName,
            "Password123!");
            
        ICommandHandler<RegisterUserCommand, Guid> handler = ServiceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand, Guid>>();
        await handler.Handle(command1, CancellationToken.None);
        
        var command2 = new RegisterUserCommand(
            email,
            _faker.Person.FirstName,
            _faker.Person.LastName,
            "Password123!");

        // Act
        Result<Guid> result = await handler.Handle(command2, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(UserErrors.EmailNotUnique);
    }
} 
