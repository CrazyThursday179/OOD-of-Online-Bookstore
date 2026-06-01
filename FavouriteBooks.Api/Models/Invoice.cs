using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Models;

public class Invoice
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssuedUtc { get; set; }
    public DateTime DueUtc { get; set; }
    public Guid ShipmentMethodId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Total { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public void MarkAsPaid()
    {
        PaymentStatus = PaymentStatus.Paid;
    }

    public void MarkAsDeclined()
    {
        PaymentStatus = PaymentStatus.Declined;
    }

    public void MarkAsValidationFailed()
    {
        PaymentStatus = PaymentStatus.ValidationFailed;
    }
}
