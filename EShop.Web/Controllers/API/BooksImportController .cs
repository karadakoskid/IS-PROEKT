using EShop.Domain.Domain_Models;
using EShop.Service.Implementation;
using EShop.Service.Interface;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksImportController : ControllerBase
    {
        private readonly IDataFetchService _dataFetchService;
        private readonly IProductService _productService;

        public BooksImportController(IDataFetchService dataFetchService, IProductService productService)
        {
            _dataFetchService = dataFetchService;
            _productService = productService;
        }

        [HttpGet("import")]
        public async Task<IActionResult> ImportBooks()
        {
            var products = await _dataFetchService.FetchCoursesFromApi();
            return Ok(products.Count);
        }

        [HttpGet("all")]
        public IActionResult GetAllBooks()
        {
            List<Product> products = _productService.GetAll();
            return Ok(products);
        }
        [HttpGet("reset")]
        [HttpPost("reset")]
        public async Task<IActionResult> ResetProducts()
        {
            // Delete all products from DB
            _productService.DeleteAllProducts();

            // Fetch fresh products from API and save
            var products = await _dataFetchService.FetchCoursesFromApi();

            return Ok(new { Count = products.Count, Message = "Database cleared and products re-imported." });
        }
    }
}
