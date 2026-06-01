using FavouriteBooks.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[Route("api/shipments")]
public class ShipmentsController(IShipmentService shipmentService) : ApiControllerBase
{
    [HttpGet("{shipmentId:guid}")]
    public async Task<IActionResult> GetShipment(Guid shipmentId)
    {
        var result = await shipmentService.GetShipmentAsync(shipmentId);
        return FromResult(result);
    }
}
