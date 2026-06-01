using FavouriteBooks.Api.DTOs.Payments;
using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Services.Payments;

public class CreditDebitPayment : PaymentMethod
{
    public override PaymentMethodType MethodType => PaymentMethodType.CreditDebit;

    protected override IReadOnlyList<string> Validate(PaymentRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CardHolderName))
        {
            errors.Add("Card holder name is required.");
        }

        var cardNumber = request.CardNumber?.Trim() ?? string.Empty;
        if (cardNumber.Length < 12 || cardNumber.Length > 19 || !cardNumber.All(char.IsDigit))
        {
            errors.Add("Card number must contain 12 to 19 digits.");
        }

        if (string.IsNullOrWhiteSpace(request.ExpiryMonth) || string.IsNullOrWhiteSpace(request.ExpiryYear))
        {
            errors.Add("Expiry month and year are required.");
        }

        var cvc = request.Cvc?.Trim() ?? string.Empty;
        if (cvc.Length < 3 || cvc.Length > 4 || !cvc.All(char.IsDigit))
        {
            errors.Add("CVC must contain 3 or 4 digits.");
        }

        return errors;
    }

    protected override string? ResolvePayerEmail(PaymentRequest request) => null;

    protected override string? ResolveMaskedAccount(PaymentRequest request)
    {
        var cardNumber = request.CardNumber?.Trim() ?? string.Empty;
        return cardNumber.Length >= 4 ? $"**** **** **** {cardNumber[^4..]}" : null;
    }
}
