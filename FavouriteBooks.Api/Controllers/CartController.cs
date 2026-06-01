using FavouriteBooks.Api.DTOs.Cart;
using FavouriteBooks.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[Route("api/carts")]
public class CartController(ICartService cartService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCart([FromQuery] Guid? customerId, [FromQuery] string? sessionId)
    {
        var result = await cartService.GetCartAsync(customerId, sessionId);
        return FromResult(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request)
    {
        var result = await cartService.AddItemAsync(request);
        return FromResult(result);
    }

    [HttpPut("items/{bookId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid bookId, [FromBody] UpdateCartItemRequest request)
    {
        var result = await cartService.UpdateItemQuantityAsync(bookId, request);
        return FromResult(result);
    }

    [HttpDelete("items/{bookId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid bookId, [FromQuery] Guid? customerId, [FromQuery] string? sessionId)
    {
        var result = await cartService.RemoveItemAsync(customerId, sessionId, bookId);
        return FromResult(result);
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart([FromQuery] Guid? customerId, [FromQuery] string? sessionId)
    {
        var result = await cartService.ClearCartAsync(customerId, sessionId);
        return FromResult(result);
    }
}
