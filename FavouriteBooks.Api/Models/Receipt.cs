using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.Models;

public class Receipt
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid InvoiceId { get; set; }
    public Guid PaymentDetailsId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public PaymentMethodType PaymentMethodType { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaidUtc { get; set; }
}
