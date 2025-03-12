namespace EuroMotors.Api.Controllers.Users;

public sealed record UpdateUserInformationRequest(string Firstname, string LastName, string PhoneNumber, string City);
