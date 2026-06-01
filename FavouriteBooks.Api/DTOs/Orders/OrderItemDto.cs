namespace FavouriteBooks.Api.DTOs.Orders;

public class OrderItemDto
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
