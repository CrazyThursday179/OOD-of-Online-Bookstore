using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Services.Payments;

public class PaymentStrategyFactory(
    CreditDebitPayment creditDebitPayment,
    PayPalPayment payPalPayment,
    AfterpayPayment afterpayPayment)
{
    public PaymentMethod GetStrategy(PaymentMethodType methodType) => methodType switch
    {
        PaymentMethodType.CreditDebit => creditDebitPayment,
        PaymentMethodType.PayPal => payPalPayment,
        PaymentMethodType.Afterpay => afterpayPayment,
        _ => throw new ArgumentOutOfRangeException(nameof(methodType), methodType, "Unsupported payment method.")
    };
}
