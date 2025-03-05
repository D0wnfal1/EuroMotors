using EuroMotors.Domain.UnitTests.Infrastructure;
using EuroMotors.Domain.Users;
using EuroMotors.Domain.Users.Events;
using Shouldly;

namespace EuroMotors.Domain.UnitTests.Users;

public class UserTests : BaseTest
{
    [Fact]
    public void Create_Should_SetPropertyValue()
    {
        // Act
        var user = User.Create(UserData.Email, UserData.FirstName, UserData.LastName, UserData.Password);

        // Assert
        user.Email.ShouldBe(UserData.Email);
        user.FirstName.ShouldBe(UserData.FirstName);
        user.LastName.ShouldBe(UserData.LastName);
        user.PasswordHash.ShouldBe(UserData.Password);
    }

    [Fact]
    public void Create_Should_RaiseUserCreatedDomainEvent()
    {
        // Act
        var user = User.Create(UserData.FirstName, UserData.LastName, UserData.Email, UserData.Password);

        // Assert
        UserRegisteredDomainEvent userCreatedDomainEvent = AssertDomainEventWasPublished<UserRegisteredDomainEvent>(user);

        userCreatedDomainEvent.UserId.ShouldBe(user.Id);
    }

}
