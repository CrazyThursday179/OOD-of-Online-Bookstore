using FavouriteBooks.Api.DTOs.Payments;
using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Services.Payments;

public class AfterpayPayment : PaymentMethod
{
    public override PaymentMethodType MethodType => PaymentMethodType.Afterpay;

    protected override IReadOnlyList<string> Validate(PaymentRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.AfterpayEmail) || !request.AfterpayEmail.Contains('@'))
        {
            errors.Add("A valid Afterpay email is required.");
        }

        if (string.IsNullOrWhiteSpace(request.AfterpayMobile))
        {
            errors.Add("An Afterpay mobile number is required.");
        }

        return errors;
    }

    protected override string? ResolvePayerEmail(PaymentRequest request) => request.AfterpayEmail?.Trim();

    protected override string? ResolveMaskedAccount(PaymentRequest request)
    {
        var mobile = request.AfterpayMobile?.Trim();
        return string.IsNullOrWhiteSpace(mobile) ? null : $"Afterpay:{mobile}";
    }
}
