using FavouriteBooks.Api.DTOs.Orders;
using FavouriteBooks.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[Route("api/orders")]
public class OrdersController(IOrderService orderService) : ApiControllerBase
{
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var result = await orderService.CheckoutAsync(request);
        return FromResult(result);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var result = await orderService.GetOrderAsync(orderId);
        return FromResult(result);
    }

    [HttpGet("invoice/{invoiceId:guid}")]
    public async Task<IActionResult> GetInvoice(Guid invoiceId)
    {
        var result = await orderService.GetInvoiceAsync(invoiceId);
        return FromResult(result);
    }
}
