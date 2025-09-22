using EShop.Domain.Domain_Models;
using EShop.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using EShop.Domain.DTO;

namespace EShop.Service.Implementation
{
    public class DataFetchService : IDataFetchService
    {
        private readonly HttpClient _httpClient;
        private readonly IProductService productService;

        public DataFetchService(IHttpClientFactory httpClientFactory, IProductService productService)
        {
            _httpClient = httpClientFactory.CreateClient();
            this.productService = productService;
        }


        //public async Task<List<Product>> FetchCoursesFromApi()
        //{
        //    var productDto = await _httpClient.GetFromJsonAsync<List<ProductDTO>>("https://www.googleapis.com/books/v1/volumes?q=harry+potter&key=AIzaSyBofIhTw5yiJf5AAUi3JuMP8Ga1NbsALQs\r\n");
        //    var products = productDto.Select(x => new Product()
        //    {
        //        Id = Guid.NewGuid(),
        //        ProductName = x.ProductName,
        //        ProductDescription = x.ProductDescription,
        //        ProductImage = x.ProductImage,
        //        ProductPrice=x.ProductPrice,
        //    }).ToList();
        //    productService.InsertMany(products);
        //    return products;

        //}
        //public async Task<List<Product>> FetchCoursesFromApi()
        //{
        //    var response = await _httpClient.GetFromJsonAsync<GoogleBooksResponse>(
        //        "https://www.googleapis.com/books/v1/volumes?q=harry+potter&key=AIzaSyBofIhTw5yiJf5AAUi3JuMP8Ga1NbsALQs");

        //    var products = response.Items?.Select(x => new Product
        //    {
        //        Id = Guid.NewGuid(),
        //        ProductName = x.VolumeInfo.Title,
        //        ProductDescription = string.Join(", ", x.VolumeInfo.Authors ?? new List<string>()),
        //        ProductImage = x.VolumeInfo.ImageLinks?.Thumbnail,
        //        ProductPrice = (double)((decimal?)(x.SaleInfo?.ListPrice?.Amount) ?? 0m)


        //    }).ToList() ?? new List<Product>();

        //    productService.InsertMany(products);
        //    return products;
        //}
        //public async Task<List<Product>> FetchCoursesFromApi()
        //{
        //    string apiKey = "AIzaSyBofIhTw5yiJf5AAUi3JuMP8Ga1NbsALQs";
        //    string searchTerm = "harry+potter";
        //    int totalBooksToFetch = 100;
        //    int maxPerRequest = 40;
        //    var random = new Random();

        //    List<Product> allProducts = new();

        //    for (int startIndex = 0; startIndex < totalBooksToFetch; startIndex += maxPerRequest)
        //    {
        //        string url = $"https://www.googleapis.com/books/v1/volumes?q={searchTerm}&startIndex={startIndex}&maxResults={maxPerRequest}&key={apiKey}";

        //        var response = await _httpClient.GetFromJsonAsync<GoogleBooksResponse>(url);

        //        var products = response?.Items?.Select(x => new Product
        //        {
        //            Id = Guid.NewGuid(),
        //            ProductName = x.VolumeInfo?.Title ?? "Unknown Title",
        //            ProductDescription = string.Join(", ", x.VolumeInfo?.Authors ?? new List<string> { "Unknown" }),
        //            ProductImage = x.VolumeInfo?.ImageLinks?.Thumbnail,
        //            ProductPrice = (double)((decimal?)(x.SaleInfo?.ListPrice?.Amount) ?? 0m)
        //        }).ToList();

        //        if (products != null)
        //            allProducts.AddRange(products);

        //        // Google may return less than requested; break if that happens
        //        if (response?.Items?.Count < maxPerRequest)
        //            break;
        //    }

        //    productService.InsertMany(allProducts);
        //    return allProducts;
        //}
        public async Task<List<Product>> FetchCoursesFromApi()
        {
            string apiKey = "AIzaSyBofIhTw5yiJf5AAUi3JuMP8Ga1NbsALQs";
            string searchTerm = "harry+potter";
            int totalBooksToFetch = 100;
            int maxPerRequest = 40;

            List<Product> allProducts = new();

            var random = new Random(); // create a Random instance

            for (int startIndex = 0; startIndex < totalBooksToFetch; startIndex += maxPerRequest)
            {
                string url = $"https://www.googleapis.com/books/v1/volumes?q={searchTerm}&startIndex={startIndex}&maxResults={maxPerRequest}&key={apiKey}";

                var response = await _httpClient.GetFromJsonAsync<GoogleBooksResponse>(url);

                var products = response?.Items?.Select(x => new Product
                {
                    Id = Guid.NewGuid(),
                    ProductName = x.VolumeInfo?.Title ?? "Unknown Title",
                    ProductDescription = string.Join(", ", x.VolumeInfo?.Authors ?? new List<string> { "Unknown" }),
                    ProductImage = x.VolumeInfo?.ImageLinks?.Thumbnail,
                    ProductPrice = (double)random.Next(100, 251) // Random number between 100 and 250 inclusive
                }).ToList();

                if (products != null)
                    allProducts.AddRange(products);

                if (response?.Items?.Count < maxPerRequest)
                    break;
            }

            productService.InsertMany(allProducts);
            return allProducts;
        }

    }
}
