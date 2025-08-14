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

        public async Task<IActionResult> Index(string searchQuery)
        {
            // Lấy sản phẩm từ bảng Product và lọc theo tên nếu có searchQuery
            var products = string.IsNullOrEmpty(searchQuery)
                ? await _context.Products.ToListAsync()
                : await _context.Products
                    .Where(p => p.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                    .ToListAsync();

            // Lấy toàn bộ sản phẩm từ bảng Product
            ViewBag.SearchQuery = searchQuery;
            return View(products);
        }
    }
}
