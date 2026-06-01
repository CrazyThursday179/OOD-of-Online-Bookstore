namespace FavouriteBooks.Api.DTOs.Catalogue;

public class BookDto
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
    public IReadOnlyList<Guid> CategoryIds { get; set; } = [];
}
