using Bogus;
using EuroMotors.Application.Abstractions.Messaging;
using EuroMotors.Application.IntegrationTests.Abstractions;
using EuroMotors.Application.Users.Login;
using EuroMotors.Application.Users.Register;
using EuroMotors.Domain.Abstractions;
using EuroMotors.Domain.Users;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace EuroMotors.Application.IntegrationTests.Users;

public class LoginUserTests : BaseIntegrationTest
{
    private readonly Faker _faker = new();
    private const string Password = "Password123!";

    public LoginUserTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task Should_LoginUser_Successfully()
    {
        // Arrange
        await CleanDatabaseAsync();

        string email = _faker.Internet.Email();

        var registerCommand = new RegisterUserCommand(
            email,
            _faker.Person.FirstName,
            _faker.Person.LastName,
            Password);

        ICommandHandler<RegisterUserCommand, Guid> registerHandler = ServiceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand, Guid>>();
        await registerHandler.Handle(registerCommand, CancellationToken.None);

        using IServiceScope scope = Factory.Services.CreateScope();
        var loginCommand = new LoginUserCommand(email, Password);

        // Act
        ICommandHandler<LoginUserCommand, AuthenticationResponse> loginHandler =
            scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginUserCommand, AuthenticationResponse>>();
        Result<AuthenticationResponse> result = await loginHandler.Handle(loginCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.AccessToken.ShouldNotBeNullOrEmpty();
        result.Value.RefreshToken.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenUserDoesNotExist()
    {
        // Arrange
        await CleanDatabaseAsync();

        using IServiceScope scope = Factory.Services.CreateScope();
        var loginCommand = new LoginUserCommand("nonexistent@example.com", Password);

        // Act
        ICommandHandler<LoginUserCommand, AuthenticationResponse> loginHandler =
            scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginUserCommand, AuthenticationResponse>>();
        Result<AuthenticationResponse> result = await loginHandler.Handle(loginCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Should_ReturnFailure_WhenPasswordIsIncorrect()
    {
        // Arrange
        await CleanDatabaseAsync();

        string email = _faker.Internet.Email();

        var registerCommand = new RegisterUserCommand(
            email,
            _faker.Person.FirstName,
            _faker.Person.LastName,
            Password);

        ICommandHandler<RegisterUserCommand, Guid> registerHandler = ServiceProvider.GetRequiredService<ICommandHandler<RegisterUserCommand, Guid>>();
        await registerHandler.Handle(registerCommand, CancellationToken.None);

        using IServiceScope scope = Factory.Services.CreateScope();
        var loginCommand = new LoginUserCommand(email, "WrongPassword123!");

        // Act
        ICommandHandler<LoginUserCommand, AuthenticationResponse> loginHandler =
            scope.ServiceProvider.GetRequiredService<ICommandHandler<LoginUserCommand, AuthenticationResponse>>();
        Result<AuthenticationResponse> result = await loginHandler.Handle(loginCommand, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(UserErrors.InvalidCredentials);
    }
}
