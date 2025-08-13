using CoffeeShopWeb.Models;
using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Policy = "Adminonly")]
    public class CoffeeShopMenuManagementController : Controller
    {
        private readonly ICoffeeShopMenuService _menuService;

        public CoffeeShopMenuManagementController(ICoffeeShopMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _menuService.GetProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _menuService.GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                await _menuService.ResetProductIdSequenceAsync();
                await _menuService.AddProductAsync(product, image);
                TempData["Success"] = "Product added";
                return RedirectToAction("Index");
            }
            ViewBag.Categories = await _menuService.GetCategoriesAsync();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _menuService.GetProductAsync(id);
            if (product == null) return NotFound();
            ViewBag.Categories = await _menuService.GetCategoriesAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile image)
        {
            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (image == null)
                {
                    ModelState.AddModelError("image", "Image is required to save the edit.");
                    ViewBag.Categories = await _menuService.GetCategoriesAsync();
                    return View(product);
                }

                var existingProduct = await _menuService.GetProductAsync(id);
                if (existingProduct == null) return NotFound();

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.IsAvailable = product.IsAvailable;

                await _menuService.UpdateProductAsync(existingProduct, image);
                TempData["Success"] = "Product updated";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await _menuService.GetCategoriesAsync();
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _menuService.GetProductAsync(id);
            if (product == null)
            {
                // Instead of 404, redirect to Index if deleted already
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Index");
            }
            return View(product);
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string decision)
        {
            if (decision == "Yes")
            {
                await _menuService.DeleteProductAsync(id);
                TempData["Success"] = "Product deleted";
            }
            return RedirectToAction("Index");
        }
    }
}