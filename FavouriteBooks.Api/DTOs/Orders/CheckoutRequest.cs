using FavouriteBooks.Api.Models;

namespace FavouriteBooks.Api.DTOs.Orders;

public class CheckoutRequest
{
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public string? GuestEmail { get; set; }
    public Guid ShipmentMethodId { get; set; }
    public Address DeliveryAddress { get; set; } = new();
}
