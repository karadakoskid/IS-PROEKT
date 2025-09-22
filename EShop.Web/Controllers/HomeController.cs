using EShop.Domain;
using EShop.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace EShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public IActionResult Index()
        {
            // Get featured products for the home page (first 6 products for carousel)
            var featuredProducts = _productService.GetAll()?.Take(6).ToList();
            
            // If no products in database, create some sample books
            if (featuredProducts == null || !featuredProducts.Any())
            {
                featuredProducts = GetSampleBooks();
            }
            
            // Check if user just logged in and had a product they wanted to add
            if (TempData["ProductToAdd"] != null && User.Identity.IsAuthenticated)
            {
                var productId = (Guid)TempData["ProductToAdd"];
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(userId))
                {
                    try
                    {
                        var product = _productService.GetById(productId);
                        if (product != null)
                        {
                            _productService.AddProductToSoppingCart(productId, Guid.Parse(userId), 1);
                            TempData["SuccessMessage"] = $"Welcome! '{product.ProductName}' has been added to your cart.";
                        }
                    }
                    catch (Exception)
                    {
                        TempData["ErrorMessage"] = "There was an error adding the item to your cart.";
                    }
                }
            }
            
            return View(featuredProducts);
        }

        private List<EShop.Domain.Domain_Models.Product> GetSampleBooks()
        {
            return new List<EShop.Domain.Domain_Models.Product>
            {
                new EShop.Domain.Domain_Models.Product
                {
                    Id = Guid.NewGuid(),
                    ProductName = "Harry Potter and the Philosopher's Stone",
                    ProductDescription = "The first book in the Harry Potter series by J.K. Rowling. Follow Harry's magical journey at Hogwarts.",
                    ProductImage = "https://m.media-amazon.com/images/I/5165He67NEL.jpg",
                    ProductPrice = 12.99,
                    Rating = 5
                },
                new EShop.Domain.Domain_Models.Product
                {
                    Id = Guid.NewGuid(),
                    ProductName = "Harry Potter and the Chamber of Secrets",
                    ProductDescription = "The second book in the Harry Potter series. Harry returns to Hogwarts for his second year.",
                    ProductImage = "https://m.media-amazon.com/images/I/51jNORv6nQL.jpg",
                    ProductPrice = 13.99,
                    Rating = 5
                },
                new EShop.Domain.Domain_Models.Product
                {
                    Id = Guid.NewGuid(),
                    ProductName = "Harry Potter and the Prisoner of Azkaban",
                    ProductDescription = "The third book in the Harry Potter series. Harry learns about his past and faces new challenges.",
                    ProductImage = "https://m.media-amazon.com/images/I/51IiQ4r35QL.jpg",
                    ProductPrice = 14.99,
                    Rating = 5
                },
                new EShop.Domain.Domain_Models.Product
                {
                    Id = Guid.NewGuid(),
                    ProductName = "The Lord of the Rings: Fellowship",
                    ProductDescription = "The first volume of Tolkien's epic fantasy trilogy. Join Frodo on his quest to destroy the Ring.",
                    ProductImage = "https://m.media-amazon.com/images/I/51Dd6aOl1KL.jpg",
                    ProductPrice = 16.99,
                    Rating = 5
                },
                new EShop.Domain.Domain_Models.Product
                {
                    Id = Guid.NewGuid(),
                    ProductName = "The Hobbit",
                    ProductDescription = "Tolkien's classic tale of Bilbo Baggins and his unexpected adventure with dwarves and a dragon.",
                    ProductImage = "https://m.media-amazon.com/images/I/51M7XGLQTBL.jpg",
                    ProductPrice = 11.99,
                    Rating = 4
                },
                new EShop.Domain.Domain_Models.Product
                {
                    Id = Guid.NewGuid(),
                    ProductName = "The Chronicles of Narnia",
                    ProductDescription = "C.S. Lewis's beloved fantasy series. Enter the magical world of Narnia through the wardrobe.",
                    ProductImage = "https://m.media-amazon.com/images/I/51cTUFQHc3L.jpg",
                    ProductPrice = 15.99,
                    Rating = 4
                }
            };
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
