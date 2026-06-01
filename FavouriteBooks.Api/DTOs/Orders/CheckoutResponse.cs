namespace FavouriteBooks.Api.DTOs.Orders;

public class CheckoutResponse
{
    public OrderDto Order { get; set; } = new();
    public InvoiceDto Invoice { get; set; } = new();
}
