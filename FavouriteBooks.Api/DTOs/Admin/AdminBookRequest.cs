namespace FavouriteBooks.Api.DTOs.Admin;

public class AdminBookRequest
{
    public string Isbn { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Format { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public List<Guid> CategoryIds { get; set; } = [];
}
