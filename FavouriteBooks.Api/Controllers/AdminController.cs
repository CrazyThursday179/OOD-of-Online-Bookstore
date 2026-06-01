using FavouriteBooks.Api.DTOs.Admin;
using FavouriteBooks.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[Route("api/admin")]
public class AdminController(IAdminService adminService) : ApiControllerBase
{
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await adminService.GetOrdersAsync();
        return Ok(orders);
    }

    [HttpPost("books")]
    public async Task<IActionResult> AddBook([FromBody] AdminBookRequest request)
    {
        var result = await adminService.AddBookAsync(request);
        return FromResult(result);
    }

    [HttpPatch("books/{bookId:guid}/stock")]
    public async Task<IActionResult> UpdateBookStock(Guid bookId, [FromBody] UpdateBookStockRequest request)
    {
        var result = await adminService.UpdateBookStockAsync(bookId, request);
        return FromResult(result);
    }

    [HttpPatch("shipments/{shipmentId:guid}/status")]
    public async Task<IActionResult> UpdateShipmentStatus(Guid shipmentId, [FromBody] UpdateShipmentStatusRequest request)
    {
        var result = await adminService.UpdateShipmentStatusAsync(shipmentId, request);
        return FromResult(result);
    }
}
