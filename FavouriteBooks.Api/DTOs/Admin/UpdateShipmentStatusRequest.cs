using FavouriteBooks.Api.Models.Enums;

namespace FavouriteBooks.Api.DTOs.Admin;

public class UpdateShipmentStatusRequest
{
    public ShipmentStatus Status { get; set; }
}
