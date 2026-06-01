namespace FavouriteBooks.Api.Models.Enums;

public enum OrderStatus
{
    PendingCheckout = 1,
    PendingPayment = 2,
    PaymentFailed = 3,
    Paid = 4,
    ShipmentCreated = 5,
    Dispatched = 6,
    Delivered = 7
}
