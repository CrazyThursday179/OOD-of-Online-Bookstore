namespace FavouriteBooks.Api.Models;

public class CartItem
{
    public Guid BookId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceSnapshot { get; set; }

    public void UpdateQuantity(int quantity, decimal unitPriceSnapshot)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        }

        Quantity = quantity;
        UnitPriceSnapshot = unitPriceSnapshot;
    }

    public decimal CalculateLineTotal() => Quantity * UnitPriceSnapshot;
}
