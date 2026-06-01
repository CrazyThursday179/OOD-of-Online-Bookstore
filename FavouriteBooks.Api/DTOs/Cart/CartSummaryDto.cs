namespace FavouriteBooks.Api.DTOs.Cart;

public class CartSummaryDto
{
    public Guid CartId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public IReadOnlyList<CartItemDto> Items { get; set; } = [];
    public decimal Subtotal { get; set; }
    public int TotalQuantity { get; set; }
    public DateTime LastUpdatedUtc { get; set; }
}
