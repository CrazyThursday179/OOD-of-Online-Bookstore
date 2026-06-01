using FavouriteBooks.Api.Common;
using FavouriteBooks.Api.DTOs.Payments;
using FavouriteBooks.Api.Models;
using FavouriteBooks.Api.Models.Enums;
using FavouriteBooks.Api.Repositories;
using FavouriteBooks.Api.Services.Payments;

namespace FavouriteBooks.Api.Services;

public class PaymentService(
    InvoiceRepository invoiceRepository,
    OrderRepository orderRepository,
    PaymentDetailsRepository paymentDetailsRepository,
    ReceiptRepository receiptRepository,
    ShipmentRepository shipmentRepository,
    PaymentStrategyFactory paymentStrategyFactory) : IPaymentService
{
    public async Task<Result<PaymentResponse>> SubmitPaymentAsync(PaymentRequest request)
    {
        var invoices = await invoiceRepository.GetAllAsync();
        var invoice = invoices.FirstOrDefault(item => item.Id == request.InvoiceId);
        if (invoice is null)
        {
            return Result<PaymentResponse>.Failure("Invoice not found.", "Invoice must exist before payment.");
        }

        var orders = await orderRepository.GetAllAsync();
        var order = orders.FirstOrDefault(item => item.Id == invoice.OrderId);
        if (order is null)
        {
            return Result<PaymentResponse>.Failure("Order not found.", "Invoice is not linked to a valid order.");
        }

        if (invoice.PaymentStatus == PaymentStatus.Paid || order.ReceiptId.HasValue)
        {
            return Result<PaymentResponse>.Failure(
                "Payment already completed.",
                "This invoice has already been paid.");
        }

        var strategy = paymentStrategyFactory.GetStrategy(request.PaymentMethodType);
        var attemptResult = strategy.Process(invoice, request);

        var paymentRecord = new PaymentDetails
        {
            Id = Guid.NewGuid(),
            InvoiceId = invoice.Id,
            Amount = invoice.Total,
            PaymentMethodType = request.PaymentMethodType,
            AttemptedUtc = DateTime.UtcNow,
            Status = attemptResult.IsSuccess && attemptResult.Data is not null
                ? attemptResult.Data.Status
                : PaymentStatus.ValidationFailed,
            ExternalReference = attemptResult.Data?.ExternalReference,
            FailureReason = attemptResult.IsSuccess ? attemptResult.Data?.FailureReason : string.Join("; ", attemptResult.Errors),
            PayerEmail = attemptResult.Data?.PayerEmail,
            MaskedAccount = attemptResult.Data?.MaskedAccount,
            SimulationMode = request.SimulationMode
        };

        var payments = await paymentDetailsRepository.GetAllAsync();
        payments.Add(paymentRecord);

        if (!attemptResult.IsSuccess || attemptResult.Data is null)
        {
            order.MarkPaymentFailed();
            invoice.MarkAsValidationFailed();

            await paymentDetailsRepository.SaveAllAsync(payments);
            await orderRepository.SaveAllAsync(orders);
            await invoiceRepository.SaveAllAsync(invoices);

            return Result<PaymentResponse>.Failure(
                "Payment validation failed.",
                attemptResult.Errors.ToArray());
        }

        if (attemptResult.Data.Status == PaymentStatus.Declined)
        {
            order.MarkPaymentFailed();
            invoice.MarkAsDeclined();

            await paymentDetailsRepository.SaveAllAsync(payments);
            await orderRepository.SaveAllAsync(orders);
            await invoiceRepository.SaveAllAsync(invoices);

            return Result<PaymentResponse>.Success(
                new PaymentResponse
                {
                    PaymentId = paymentRecord.Id,
                    InvoiceId = invoice.Id,
                    Status = PaymentStatus.Declined,
                    Message = paymentRecord.FailureReason ?? "Payment declined.",
                    ExternalReference = paymentRecord.ExternalReference,
                    Order = order.ToDto()
                },
                "Payment declined.");
        }

        invoice.MarkAsPaid();
        order.MarkPaid();

        var receipt = new Receipt
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            InvoiceId = invoice.Id,
            PaymentDetailsId = paymentRecord.Id,
            ReceiptNumber = $"RCT-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
            PaymentMethodType = request.PaymentMethodType,
            AmountPaid = invoice.Total,
            PaidUtc = DateTime.UtcNow
        };

        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            OrderId = order.Id,
            ReceiptId = receipt.Id,
            ShipmentMethodId = order.ShipmentMethodId,
            DeliveryAddress = order.DeliveryAddress,
            Status = ShipmentStatus.ReadyForDispatch,
            TrackingCode = $"TRK-{Guid.NewGuid():N}"[..16],
            CreatedUtc = DateTime.UtcNow
        };

        order.AttachReceipt(receipt.Id);
        order.AttachShipment(shipment.Id);

        var receipts = await receiptRepository.GetAllAsync();
        var shipments = await shipmentRepository.GetAllAsync();
        receipts.Add(receipt);
        shipments.Add(shipment);

        await paymentDetailsRepository.SaveAllAsync(payments);
        await receiptRepository.SaveAllAsync(receipts);
        await shipmentRepository.SaveAllAsync(shipments);
        await orderRepository.SaveAllAsync(orders);
        await invoiceRepository.SaveAllAsync(invoices);

        return Result<PaymentResponse>.Success(
            new PaymentResponse
            {
                PaymentId = paymentRecord.Id,
                InvoiceId = invoice.Id,
                Status = PaymentStatus.Paid,
                Message = "Payment processed successfully. Receipt and shipment created.",
                ExternalReference = paymentRecord.ExternalReference,
                Receipt = receipt.ToDto(),
                Shipment = shipment.ToDto(),
                Order = order.ToDto()
            },
            "Payment processed successfully.");
    }

    public async Task<Result<ReceiptDto>> GetReceiptAsync(Guid receiptId)
    {
        var receipts = await receiptRepository.GetAllAsync();
        var receipt = receipts.FirstOrDefault(item => item.Id == receiptId);

        return receipt is null
            ? Result<ReceiptDto>.Failure("Receipt not found.", $"No receipt exists for id '{receiptId}'.")
            : Result<ReceiptDto>.Success(receipt.ToDto(), "Receipt retrieved.");
    }
}
