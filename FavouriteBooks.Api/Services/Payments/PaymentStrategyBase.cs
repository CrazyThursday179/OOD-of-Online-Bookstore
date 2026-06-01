using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Payments;
using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Services.Payments;

public abstract class PaymentMethod
{
    public abstract PaymentMethodType MethodType { get; }

    public Result<PaymentAttempt> Process(Invoice invoice, PaymentRequest request)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return Result<PaymentAttempt>.Failure("Payment validation failed.", validationErrors.ToArray());
        }

        var normalizedMode = NormalizeSimulationMode(request.SimulationMode);
        if (normalizedMode is null)
        {
            return Result<PaymentAttempt>.Failure(
                "Invalid simulation mode.",
                "Simulation mode must be Success, Declined, or omitted.");
        }

        if (string.Equals(normalizedMode, "Declined", StringComparison.OrdinalIgnoreCase))
        {
            return Result<PaymentAttempt>.Success(
                new PaymentAttempt
                {
                    Amount = invoice.Total,
                    MethodType = MethodType,
                    Status = PaymentStatus.Declined,
                    FailureReason = "Payment was declined by the simulated payment provider.",
                    ExternalReference = $"SIM-{MethodType}-{Guid.NewGuid():N}"[..18],
                    PayerEmail = ResolvePayerEmail(request),
                    MaskedAccount = ResolveMaskedAccount(request),
                    SimulationMode = normalizedMode
                },
                "Payment was declined by the simulator.");
        }

        return Result<PaymentAttempt>.Success(
            new PaymentAttempt
            {
                Amount = invoice.Total,
                MethodType = MethodType,
                Status = PaymentStatus.Paid,
                ExternalReference = $"SIM-{MethodType}-{Guid.NewGuid():N}"[..18],
                PayerEmail = ResolvePayerEmail(request),
                MaskedAccount = ResolveMaskedAccount(request),
                SimulationMode = normalizedMode ?? "Success"
            },
            "Payment processed successfully.");
    }

    protected abstract IReadOnlyList<string> Validate(PaymentRequest request);
    protected abstract string? ResolvePayerEmail(PaymentRequest request);
    protected abstract string? ResolveMaskedAccount(PaymentRequest request);

    private static string? NormalizeSimulationMode(string? simulationMode)
    {
        if (string.IsNullOrWhiteSpace(simulationMode))
        {
            return "Success";
        }

        return simulationMode.Trim().ToLowerInvariant() switch
        {
            "success" => "Success",
            "declined" => "Declined",
            _ => null
        };
    }
}
