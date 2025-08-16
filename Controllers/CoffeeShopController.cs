using CoffeeShopWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Controllers
{
    public class CoffeeShopController : Controller
    {
        private readonly Context _context;

        public CoffeeShopController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery, int? categoryId, decimal? minPrice, decimal? maxPrice, string sortBy = "name")
        {
            // Bắt đầu với tất cả sản phẩm có sẵn
            var query = _context.Products.Where(p => p.IsAvailable);

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                        p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            // Lọc theo category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Sắp xếp
            query = sortBy.ToLower() switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_desc" => query.OrderByDescending(p => p.Name),
                _ => query.OrderBy(p => p.Name)
            };

            var products = await query.ToListAsync();

            // Lấy danh sách categories cho dropdown
            var categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();

            // Truyền dữ liệu cho View
            ViewBag.SearchQuery = searchQuery;
            ViewBag.CategoryId = categoryId;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;
            ViewBag.Categories = categories;

            return View(products);
        }
    }
}
