using FavouriteBooks.Api.DTOs.Payments;
using FavouriteBooks.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FavouriteBooks.Api.Controllers;

[Route("api/payments")]
public class PaymentsController(IPaymentService paymentService) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SubmitPayment([FromBody] PaymentRequest request)
    {
        var result = await paymentService.SubmitPaymentAsync(request);
        return FromResult(result);
    }

    [HttpGet("receipts/{receiptId:guid}")]
    public async Task<IActionResult> GetReceipt(Guid receiptId)
    {
        var result = await paymentService.GetReceiptAsync(receiptId);
        return FromResult(result);
    }
}
