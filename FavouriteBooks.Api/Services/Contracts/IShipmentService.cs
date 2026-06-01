using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Shipments;

namespace FavouriteBooks.Api.Services;

public interface IShipmentService
{
    Task<Result<ShipmentDto>> GetShipmentAsync(Guid shipmentId);
    Task<IReadOnlyList<ShipmentMethodDto>> GetShipmentMethodsAsync();
}
