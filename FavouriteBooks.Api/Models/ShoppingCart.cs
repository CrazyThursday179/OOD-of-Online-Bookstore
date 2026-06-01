namespace FavouriteBooks.Api.Models;

public class ShoppingCart
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? SessionId { get; set; }
    public List<CartItem> Items { get; set; } = [];
    public DateTime LastUpdatedUtc { get; set; }

    public bool IsEmpty() => Items.Count == 0;

    public void AddItem(Book book, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        }

        var existingItem = Items.FirstOrDefault(item => item.BookId == book.Id);
        var targetQuantity = quantity + (existingItem?.Quantity ?? 0);

        if (!book.IsAvailable(targetQuantity))
        {
            throw new InvalidOperationException($"Requested quantity exceeds available stock for '{book.Title}'.");
        }

        if (existingItem is null)
        {
            Items.Add(new CartItem
            {
                BookId = book.Id,
                Quantity = quantity,
                UnitPriceSnapshot = book.Price
            });
        }
        else
        {
            existingItem.UpdateQuantity(targetQuantity, book.Price);
        }

        Touch();
    }

    public void RemoveItem(Guid bookId)
    {
        Items.RemoveAll(item => item.BookId == bookId);
        Touch();
    }

    public void UpdateQuantity(Book book, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be positive.");
        }

        if (!book.IsAvailable(quantity))
        {
            throw new InvalidOperationException($"Only {book.StockQuantity} units are currently available.");
        }

        var item = Items.FirstOrDefault(entry => entry.BookId == book.Id);
        if (item is null)
        {
            throw new InvalidOperationException("The selected book is not in the cart.");
        }

        item.UpdateQuantity(quantity, book.Price);
        Touch();
    }

    public decimal CalculateTotal()
    {
        return Items.Sum(item => item.CalculateLineTotal());
    }

    public void Clear()
    {
        Items.Clear();
        Touch();
    }

    private void Touch()
    {
        LastUpdatedUtc = DateTime.UtcNow;
    }
}
