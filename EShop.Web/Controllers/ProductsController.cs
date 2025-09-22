using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShop.Domain.Domain_Models;
using EShop.Service.Interface;
using Microsoft.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using EShop.Domain.DTO;

namespace EShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(_productService.GetAll());
        }

        // GET: Products/Details/5
        public IActionResult Details(Guid id)
        {
            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("ProductName,ProductDescription,ProductImage,ProductPrice,Rating")] Product product)
        {
            if (ModelState.IsValid)
            {
                _productService.Insert(product);
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public IActionResult Edit(Guid pid)
        {
            var product = _productService.GetById(pid);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [Bind("Id,ProductName,ProductDescription,ProductImage,ProductPrice,Rating")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            _productService.Update(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public IActionResult Delete(Guid id)
        {

            var product = _productService.GetById(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _productService.DeleteById(id);

            return RedirectToAction(nameof(Index));
        }


        //public IActionResult AddProductToCard(Guid id)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        //    if (userId == null)
        //    {
        //        return NotFound();
        //    }

        //    _productService.AddProductToSoppingCart(id, Guid.Parse(userId));
        //    return RedirectToAction(nameof(Index));
        //}

        public IActionResult AddProductToCard(Guid id)
        {
            AddToCartDTO model = _productService.GetSelectedShoppingCartProduct(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult AddProductToCard(AddToCartDTO model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                // Redirect to login if user is not authenticated
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            
            _productService.AddProductToSoppingCart(model.SelectedProductId, Guid.Parse(userId), model.Quantity);
            return RedirectToAction(nameof(Index));
        }

        // Simple AddToCart action for quick add from home page
        public IActionResult AddToCart(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                // Store the product they wanted to add to redirect back after login
                TempData["ProductToAdd"] = id;
                TempData["ErrorMessage"] = "Please log in to add items to your cart.";
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            
            try
            {
                // Get the product details to show in success message
                var product = _productService.GetById(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Product not found.";
                    return RedirectToAction("Index", "Home");
                }
                
                // Add product with quantity 1 by default
                _productService.AddProductToSoppingCart(id, Guid.Parse(userId), 1);
                
                TempData["SuccessMessage"] = $"'{product.ProductName}' has been added to your cart!";
                
                // Redirect to shopping cart to show the added item
                return RedirectToAction("Index", "ShoppingCarts");
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "There was an error adding the item to your cart. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
