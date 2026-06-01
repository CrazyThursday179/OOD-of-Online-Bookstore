namespace FavouriteBooks.Api.DTOs.Cart;

public class CartItemDto
{
    public Guid BookId { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
