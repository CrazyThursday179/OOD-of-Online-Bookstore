using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Payments;

namespace FavouriteBooks.Api.Services;

public interface IPaymentService
{
    Task<Result<PaymentResponse>> SubmitPaymentAsync(PaymentRequest request);
    Task<Result<ReceiptDto>> GetReceiptAsync(Guid receiptId);
}
