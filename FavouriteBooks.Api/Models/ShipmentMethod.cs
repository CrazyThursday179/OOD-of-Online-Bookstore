namespace FavouriteBooks.Api.Models;

public class ShipmentMethod
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public int EstimatedDeliveryDays { get; set; }
    public bool IsActive { get; set; }
}
