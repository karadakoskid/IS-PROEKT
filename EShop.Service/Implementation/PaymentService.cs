using EShop.Domain.DTO;
using EShop.Domain.Payment;
using EShop.Service.Interface;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace EShop.Service.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly StripeSettings _stripeSettings;
        private readonly IOrderService _orderService;

        public PaymentService(IOptions<StripeSettings> stripeSettings, IOrderService orderService)
        {
            _stripeSettings = stripeSettings.Value;
            _orderService = orderService;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<string> CreatePaymentIntentAsync(PaymentIntentCreateRequest request)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100), // Convert to cents
                Currency = request.Currency,
                PaymentMethodTypes = request.PaymentMethodTypes,
                Metadata = request.Metadata
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            
            return paymentIntent.ClientSecret;
        }

        public async Task<string> CreateCheckoutSessionAsync(CheckoutSessionCreateRequest request)
        {
            var lineItems = request.LineItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = item.PriceData.Currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.PriceData.ProductData.Name,
                        Description = item.PriceData.ProductData.Description,
                        Images = item.PriceData.ProductData.Images
                    },
                    UnitAmount = item.PriceData.UnitAmount
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = request.Mode,
                SuccessUrl = request.SuccessUrl,
                CancelUrl = request.CancelUrl,
                CustomerEmail = request.CustomerEmail,
                Metadata = request.Metadata
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            
            return session.Url;
        }

        public async Task<bool> ValidateWebhookAsync(string payload, string signature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    payload, 
                    signature, 
                    _stripeSettings.WebhookSecret
                );

                // Handle different event types
                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        var session = stripeEvent.Data.Object as Session;
                        if (session != null)
                        {
                            await HandleSuccessfulPaymentAsync(session.Id);
                        }
                        break;
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        // Handle successful payment
                        break;
                    default:
                        Console.WriteLine($"Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                return true;
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Webhook validation failed: {ex.Message}");
                return false;
            }
        }

        public async Task HandleSuccessfulPaymentAsync(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                if (session?.Metadata != null && session.Metadata.ContainsKey("order_id"))
                {
                    var orderId = session.Metadata["order_id"];
                    // Update order status to paid
                    // You can extend this to update order status in your database
                    Console.WriteLine($"Payment successful for order: {orderId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling successful payment: {ex.Message}");
                throw;
            }
        }
    }
}
