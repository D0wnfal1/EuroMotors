using EuroMotors.Domain.UnitTests.Infrastructure;
using EuroMotors.Domain.Users;
using EuroMotors.Domain.Users.Events;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Users;

public class UserTests : BaseTest
{
    private static class UserData
    {
        public const string Email = "test@example.com";
        public const string FirstName = "John";
        public const string LastName = "Doe";
        public const string Password = "hashedpassword123";
    }

    [Fact]
    public void Create_Should_SetPropertyValues()
    {
        // Act
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);

        // Assert
        user.Email.ShouldBe(UserData.Email);
        user.FirstName.ShouldBe(UserData.FirstName);
        user.LastName.ShouldBe(UserData.LastName);
        user.PasswordHash.ShouldBe(UserData.Password);
        user.Roles.Count.ShouldBe(1);
        user.Roles.ShouldContain(Role.Customer);
    }

    [Fact]
    public void Create_Should_RaiseUserRegisteredDomainEvent()
    {
        // Act
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);

        // Assert
        UserRegisteredDomainEvent domainEvent = AssertDomainEventWasPublished<UserRegisteredDomainEvent>(user);
        domainEvent.UserId.ShouldBe(user.Id);
    }

    [Fact]
    public void UpdateContactInfo_Should_UpdateUserContactInformation()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        
        string newEmail = "updated@example.com";
        string newFirstName = "Jane";
        string newLastName = "Smith";
        string newPhoneNumber = "+9876543210";
        string newCity = "Los Angeles";

        // Act
        user.UpdateContactInfo(newEmail, newFirstName, newLastName, newPhoneNumber, newCity);

        // Assert
        user.Email.ShouldBe(newEmail);
        user.FirstName.ShouldBe(newFirstName);
        user.LastName.ShouldBe(newLastName);
        user.PhoneNumber.ShouldBe(newPhoneNumber);
        user.City.ShouldBe(newCity);
    }

    [Fact]
    public void SetRefreshToken_Should_UpdateRefreshTokenProperties()
    {
        // Arrange
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);
        
        string refreshToken = "refreshtoken123";
        DateTime expiryTime = DateTime.UtcNow.AddDays(7);

        // Act
        user.SetRefreshToken(refreshToken, expiryTime);

        // Assert
        user.RefreshToken.ShouldBe(refreshToken);
        user.RefreshTokenExpiryTime.ShouldBe(expiryTime);
    }
}
