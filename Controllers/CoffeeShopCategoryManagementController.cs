using CoffeeShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CoffeeShopCategoryManagementController : Controller
    {
        private readonly Context _context;

        public CoffeeShopCategoryManagementController(Context context)
        {
            _context = context;
        }

        // GET: Category Management Index
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Categories.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var categories = await query.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Search = search;
            return View(categories);
        }

        // GET: Create Category
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                // Check if category name already exists
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
                    return View(category);
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Danh mục đã được tạo thành công.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Edit Category
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Edit Category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if category name already exists (excluding current category)
                    var existingCategory = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != id);

                    if (existingCategory != null)
                    {
                        ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
                        return View(category);
                    }

                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Danh mục đã được cập nhật thành công.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Search Category
        public async Task<IActionResult> Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction(nameof(Index));
            }

            var categories = await _context.Categories
                .Where(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Search = keyword;
            return View("Index", categories); // tái sử dụng view Index
        }

        // GET: Delete Category
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if category is being used by products
            var productCount = await _context.Products.CountAsync(p => p.CategoryId == id);
            ViewBag.ProductCount = productCount;

            return View(category);
        }

        // POST: Delete Category
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Check if category is being used by products
            var productCount = await _context.Products.CountAsync(p => p.CategoryId == id);
            if (productCount > 0)
            {
                TempData["Error"] = $"Không thể xóa danh mục này vì có {productCount} sản phẩm đang sử dụng.";
                return RedirectToAction(nameof(Index));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Danh mục đã được xóa thành công.";
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
