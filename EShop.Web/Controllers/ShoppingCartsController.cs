using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShop.Domain.Domain_Models;
using EShop.Repository;
using EShop.Service.Interface;
using System.Security.Claims;
using EShop.Service.Implementation;

namespace EShop.Web.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;

        public ShoppingCartsController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        // GET: ShoppingCarts
        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userShoppingCart = _shoppingCartService.GetByUserIdWithIncludedPrducts(Guid.Parse(userId));
            return View(userShoppingCart);
        }

        public IActionResult Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _shoppingCartService.DeleteProductFromShoppingCart(id);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult OrderNow()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(userId == null)
            {
                throw new Exception("Log in");
            }

            _shoppingCartService.OrderProducts(Guid.Parse(userId));

            return RedirectToAction("Index", "ShoppingCarts");
        }

    }
}
