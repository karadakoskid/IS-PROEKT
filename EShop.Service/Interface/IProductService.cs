using EShop.Domain.Domain_Models;
using EShop.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Service.Interface
{
    public interface IProductService
    {
        List<Product> GetAll();
        Product? GetById(Guid id);
        Product Insert(Product product);
        ICollection<Product> InsertMany(ICollection<Product> products);
        Product Update(Product product);
        Product DeleteById(Guid id);
        AddToCartDTO GetSelectedShoppingCartProduct(Guid id);
        void AddProductToSoppingCart(Guid id, Guid userId, int quantity);
        void DeleteAllProducts();
    }
}
