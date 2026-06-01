using FavouriteBooks.Api.DTOs.Orders;
using FavouriteBooks.Api.DTOs.Shipments;
using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.DTOs.Payments;

public class PaymentResponse
{
    public Guid PaymentId { get; set; }
    public Guid InvoiceId { get; set; }
    public PaymentStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public ReceiptDto? Receipt { get; set; }
    public ShipmentDto? Shipment { get; set; }
    public OrderDto? Order { get; set; }
}
