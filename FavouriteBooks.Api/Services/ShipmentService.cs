using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Shipments;
using FavouriteBooks.Api.Repositories;

namespace FavouriteBooks.Api.Services;

public class ShipmentService(
    ShipmentRepository shipmentRepository,
    ShipmentMethodRepository shipmentMethodRepository) : IShipmentService
{
    public async Task<Result<ShipmentDto>> GetShipmentAsync(Guid shipmentId)
    {
        var shipments = await shipmentRepository.GetAllAsync();
        var shipment = shipments.FirstOrDefault(item => item.Id == shipmentId);

        return shipment is null
            ? Result<ShipmentDto>.Failure("Shipment not found.", $"No shipment exists for id '{shipmentId}'.")
            : Result<ShipmentDto>.Success(shipment.ToDto(), "Shipment retrieved.");
    }

    public async Task<IReadOnlyList<ShipmentMethodDto>> GetShipmentMethodsAsync()
    {
        var methods = await shipmentMethodRepository.GetAllAsync();
        return methods
            .Where(method => method.IsActive)
            .OrderBy(method => method.Cost)
            .Select(method => method.ToDto())
            .ToList();
    }
}
