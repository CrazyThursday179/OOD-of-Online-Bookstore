using FavouriteBooks.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[Route("api/catalogue")]
public class CatalogueController(
    ICatalogueService catalogueService,
    IShipmentService shipmentService) : ApiControllerBase
{
    [HttpGet("books")]
    public async Task<IActionResult> GetBooks([FromQuery] string? search, [FromQuery] Guid? categoryId)
    {
        var books = await catalogueService.GetBooksAsync(search, categoryId);
        return Ok(books);
    }

    [HttpGet("books/{id:guid}")]
    public async Task<IActionResult> GetBookById(Guid id)
    {
        var result = await catalogueService.GetBookByIdAsync(id);
        return FromResult(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await catalogueService.GetCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("shipment-methods")]
    public async Task<IActionResult> GetShipmentMethods()
    {
        var methods = await shipmentService.GetShipmentMethodsAsync();
        return Ok(methods);
    }
}
