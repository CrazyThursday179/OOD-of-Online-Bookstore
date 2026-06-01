using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.DTOs.Payments;

public class PaymentRequest
{
    public Guid InvoiceId { get; set; }
    public PaymentMethodType PaymentMethodType { get; set; }
    public string? CardHolderName { get; set; }
    public string? CardNumber { get; set; }
    public string? ExpiryMonth { get; set; }
    public string? ExpiryYear { get; set; }
    public string? Cvc { get; set; }
    public string? PayPalEmail { get; set; }
    public string? AfterpayEmail { get; set; }
    public string? AfterpayMobile { get; set; }
    public string? SimulationMode { get; set; }
}
