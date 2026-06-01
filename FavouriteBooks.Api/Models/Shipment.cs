using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Models;

public class Shipment
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

    public void MarkDispatched()
    {
        if (Status == ShipmentStatus.Delivered)
        {
            throw new InvalidOperationException("Delivered shipment cannot be moved back to dispatched.");
        }

        Status = ShipmentStatus.Dispatched;
        DispatchedUtc ??= DateTime.UtcNow;
    }

    public void MarkDelivered()
    {
        if (Status != ShipmentStatus.Dispatched && Status != ShipmentStatus.ReadyForDispatch)
        {
            throw new InvalidOperationException("Shipment must be ready or dispatched before delivery.");
        }

        if (DispatchedUtc is null)
        {
            DispatchedUtc = DateTime.UtcNow;
        }

        Status = ShipmentStatus.Delivered;
        DeliveredUtc = DateTime.UtcNow;
    }
}
