using EShop.Domain.DTO;

namespace EShop.Service.Interface
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntentAsync(PaymentIntentCreateRequest request);
        Task<string> CreateCheckoutSessionAsync(CheckoutSessionCreateRequest request);
        Task<bool> ValidateWebhookAsync(string payload, string signature);
        Task HandleSuccessfulPaymentAsync(string sessionId);
    }
}
