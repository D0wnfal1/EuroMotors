using System.Text.Json.Serialization;

namespace EuroMotors.Api.Controllers.Carts;

public sealed class CartRequest
{
    public Guid? UserId { get;}

    public Guid? SessionId { get;} 
}
