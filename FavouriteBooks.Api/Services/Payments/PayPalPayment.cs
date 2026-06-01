using FavouriteBooks.Api.DTOs.Payments;
using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Services.Payments;

public class PayPalPayment : PaymentMethod
{
    public override PaymentMethodType MethodType => PaymentMethodType.PayPal;

    protected override IReadOnlyList<string> Validate(PaymentRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PayPalEmail) || !request.PayPalEmail.Contains('@'))
        {
            return ["A valid PayPal email is required."];
        }

        return [];
    }

    protected override string? ResolvePayerEmail(PaymentRequest request) => request.PayPalEmail?.Trim();

    protected override string? ResolveMaskedAccount(PaymentRequest request)
    {
        var email = request.PayPalEmail?.Trim();
        return string.IsNullOrWhiteSpace(email) ? null : $"PayPal:{email}";
    }
}
