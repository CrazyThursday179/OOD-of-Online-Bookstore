using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Services.Payments;

public class PaymentAttempt
{
    public decimal Amount { get; set; }
    public PaymentMethodType MethodType { get; set; }
    public PaymentStatus Status { get; set; }
    public string? FailureReason { get; set; }
    public string? ExternalReference { get; set; }
    public string? PayerEmail { get; set; }
    public string? MaskedAccount { get; set; }
    public string? SimulationMode { get; set; }
}
