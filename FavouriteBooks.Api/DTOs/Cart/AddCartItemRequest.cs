namespace FavouriteBooks.Api.DTOs.Cart;

public class AddCartItemRequest
{
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
}
