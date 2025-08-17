using CoffeeShopWeb.Models;
using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CoffeeShopResourceManagementController : Controller
    {
        private readonly IResourceService _resourceService;
        private readonly Context _context;

        public CoffeeShopResourceManagementController(IResourceService resourceService, Context context)
        {
            _resourceService = resourceService;
            _context = context;
        }

        public async Task<IActionResult> Index(string search)
        {
            var resources = await _resourceService.GetAllAsync(search);
            ViewBag.Search = search;
            return View(resources);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Resource resource)
        {
            if (ModelState.IsValid)
            {
                await _resourceService.AddAsync(resource);        
                TempData["Success"] = "Resource created successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(resource);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var resource = await _resourceService.GetByIdAsync(id);
            if (resource == null) return NotFound();
            ViewBag.Categories = _context.Categories.ToList();
            return View(resource);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Resource resource)
        {
            if (ModelState.IsValid)
            {
                await _resourceService.UpdateAsync(resource);
                TempData["Success"] = "Resource updated successfully";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _context.Categories.ToList();
            return View(resource);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var resource = await _resourceService.GetByIdAsync(id);
            if (resource == null) return NotFound();
            return View(resource);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, string decision)
        {
            if (decision == "Yes")
            {
                await _resourceService.DeleteAsync(id);
                TempData["Success"] = "Resource deleted successfully";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
