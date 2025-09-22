using EShop.Domain.DTO;
using EShop.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EShop.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IOrderService _orderService;

        public PaymentController(
            IPaymentService paymentService,
            IShoppingCartService shoppingCartService,
            IOrderService orderService)
        {
            _paymentService = paymentService;
            _shoppingCartService = shoppingCartService;
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var cart = _shoppingCartService.GetByUserIdWithIncludedPrducts(Guid.Parse(userId));
            if (cart?.Products == null || !cart.Products.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty. Please add some books before checkout.";
                return RedirectToAction("Index", "ShoppingCarts");
            }

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var cart = _shoppingCartService.GetByUserIdWithIncludedPrducts(Guid.Parse(userId));
                if (cart?.Products == null || !cart.Products.Any())
                {
                    return Json(new { success = false, message = "Cart is empty" });
                }

                // Create line items for Stripe
                var lineItems = cart.Products.Select(item => new CheckoutLineItem
                {
                    PriceData = new CheckoutPriceData
                    {
                        Currency = "usd",
                        ProductData = new CheckoutProductData
                        {
                            Name = item.Product.ProductName,
                            Description = item.Product.ProductDescription,
                            Images = !string.IsNullOrEmpty(item.Product.ProductImage) ? 
                                new List<string> { item.Product.ProductImage } : new List<string>()
                        },
                        UnitAmount = (long)(item.Product.ProductPrice * 100) // Convert to cents
                    },
                    Quantity = item.Quantity
                }).ToList();

                // Create checkout session request
                var request = new CheckoutSessionCreateRequest
                {
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = Url.Action("Success", "Payment", null, Request.Scheme) + "?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = Url.Action("Cancel", "Payment", null, Request.Scheme),
                    CustomerEmail = User.FindFirstValue(ClaimTypes.Email),
                    Metadata = new Dictionary<string, string>
                    {
                        { "user_id", userId }
                    }
                };

                var sessionUrl = await _paymentService.CreateCheckoutSessionAsync(request);
                
                return Json(new { success = true, url = sessionUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to create checkout session: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Success(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                TempData["ErrorMessage"] = "Invalid payment session.";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Handle successful payment
                await _paymentService.HandleSuccessfulPaymentAsync(session_id);
                
                // Process the order (move items from cart to order)
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!string.IsNullOrEmpty(userId))
                {
                    var result = _shoppingCartService.OrderProducts(Guid.Parse(userId));
                    if (result)
                    {
                        TempData["SuccessMessage"] = "🎉 Payment successful! Your order has been placed and you'll receive a confirmation email shortly.";
                    }
                    else
                    {
                        TempData["WarningMessage"] = "Payment was processed, but there was an issue creating your order. Please contact support.";
                    }
                }

                return View();
            }
            catch
            {
                TempData["ErrorMessage"] = "There was an error processing your payment. Please contact support.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public IActionResult Cancel()
        {
            TempData["InfoMessage"] = "Payment was canceled. Your items are still in your cart.";
            return RedirectToAction("Index", "ShoppingCarts");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            try
            {
                var isValid = await _paymentService.ValidateWebhookAsync(json, stripeSignature);
                if (isValid)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook error: {ex.Message}");
            }
        }
    }
}
