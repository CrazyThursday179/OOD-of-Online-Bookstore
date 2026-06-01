using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.DTOs.Orders;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssuedUtc { get; set; }
    public DateTime DueUtc { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}
