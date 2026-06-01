namespace FavouriteBooks.Api.DTOs.Cart;

public class UpdateCartItemRequest
{
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public int Quantity { get; set; }
}
