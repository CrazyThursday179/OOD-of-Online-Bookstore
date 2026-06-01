using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Models;

public class PaymentDetails
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethodType PaymentMethodType { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime AttemptedUtc { get; set; }
    public string? PayerEmail { get; set; }
    public string? MaskedAccount { get; set; }
    public string? ExternalReference { get; set; }
    public string? FailureReason { get; set; }
    public string? SimulationMode { get; set; }
}
