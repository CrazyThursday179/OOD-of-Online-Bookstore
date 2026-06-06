namespace FavouriteBooks.Api.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public List<Guid> CategoryIds { get; set; } = [];

    public bool IsAvailable(int requestedQuantity = 1)
    {
        return IsActive && requestedQuantity > 0 && StockQuantity >= requestedQuantity;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        }

        if (!IsAvailable(quantity))
        {
            throw new InvalidOperationException($"Insufficient stock for '{Title}'.");
        }

        StockQuantity -= quantity;
    }

    public void SetStockQuantity(int stockQuantity)
    {
        if (stockQuantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stockQuantity), "Stock quantity must be zero or greater.");
        }

        StockQuantity = stockQuantity;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
