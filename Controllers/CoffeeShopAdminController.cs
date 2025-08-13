using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShopWeb.Models; // cần để dùng Context

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CoffeeShopAdminController : Controller
    {
        private readonly Context _context;

        public CoffeeShopAdminController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy dữ liệu thống kê số sản phẩm mỗi Category
            var categoryData = await _context.Categories
                .Select(c => new
                {
                    CategoryName = c.Name,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToListAsync();

            ViewBag.CategoryChartData = categoryData;

            return View();
        }
    }
}
