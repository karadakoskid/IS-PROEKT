using EShop.Domain.Domain_Models;
using EShop.Domain.DTO;
using EShop.Repository;
using EShop.Repository.Interface;
using EShop.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Service.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductInShoppingCart> _productInShoppingCartRepository;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ApplicationDbContext _dbContext;
        public ProductService(IRepository<Product> productRepository, IRepository<ProductInShoppingCart> productInShoppingCartRepository, IShoppingCartService shoppingCartService, ApplicationDbContext dbContext)
        {
            _productRepository = productRepository;
            _productInShoppingCartRepository = productInShoppingCartRepository;
            _shoppingCartService= shoppingCartService;
            _dbContext = dbContext;
        }

        public void AddProductToSoppingCart(Guid id, Guid userId, int quantity)
        {
            var shoppingCart = _shoppingCartService.GetByUserId(userId);

            if (shoppingCart == null)
            {
                throw new Exception("Shopping cart not found");
            }

            var product = GetById(id);

            if (product == null)
            {
                throw new Exception("Product not found");
            }

            UpdateCartItem(product, shoppingCart, quantity);

        }


        private ProductInShoppingCart? GetProductInShoppingCart(Guid productId, Guid cartId)
        {
            return _productInShoppingCartRepository.Get(selector: x => x,
                predicate: x => x.ShoppingCartId.ToString() == cartId.ToString()
                                                && x.ProductId.ToString() == productId.ToString());
        }

        private void UpdateCartItem(Product product, ShoppingCart shoppingCart, int quantity)
        {
            var existingProduct = GetProductInShoppingCart(product.Id, shoppingCart.Id);

            if(existingProduct == null)
            {
                var productInShoppingCart = new ProductInShoppingCart
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    ShoppingCartId = shoppingCart.Id,
                    Product = product,
                    ShoppingCart = shoppingCart,
                    Quantity = quantity
                };

                _productInShoppingCartRepository.Insert(productInShoppingCart);
            }
            else
            {
                existingProduct.Quantity+=quantity;
                _productInShoppingCartRepository.Update(existingProduct);
            }
        }

        public Product DeleteById(Guid id)
        {
            var product = GetById(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }
            _productRepository.Delete(product);
            return product;
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll(selector: x => x).ToList();
        }

        public Product? GetById(Guid id)
        {
            return _productRepository.Get(selector: x => x,
                                          predicate: x => x.Id.Equals(id));
        }

        public Product Insert(Product product)
        {
            product.Id = Guid.NewGuid();
            return _productRepository.Insert(product);
        }
        public ICollection<Product> InsertMany(ICollection<Product> products)
        {
            return _productRepository.InsertMany(products);
        }

        public Product Update(Product product)
        {
            return _productRepository.Update(product);
        }

        public AddToCartDTO GetSelectedShoppingCartProduct(Guid id)
        {
            var selectedProduct = GetById(id);

            var addProductToCartModel = new AddToCartDTO
            {
                SelectedProductId = selectedProduct.Id,
                SelectedProductName = selectedProduct.ProductName,
                Quantity = 1
            };

            return addProductToCartModel;
        }
        public void DeleteAllProducts()
        {
            var allProducts = _dbContext.Products.ToList();
            _dbContext.Products.RemoveRange(allProducts);
            _dbContext.SaveChanges();
        }
    }
}
