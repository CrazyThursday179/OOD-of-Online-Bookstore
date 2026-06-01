using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.DTOs.Shipments;

public class ShipmentDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid ShipmentMethodId { get; set; }
    public Address DeliveryAddress { get; set; } = new();
    public ShipmentStatus Status { get; set; }
    public string TrackingCode { get; set; } = string.Empty;
    public DateTime CreatedUtc { get; set; }
    public DateTime? DispatchedUtc { get; set; }
    public DateTime? DeliveredUtc { get; set; }
}
