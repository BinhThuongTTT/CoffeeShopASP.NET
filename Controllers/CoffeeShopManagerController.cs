using CoffeeShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Policy = "ManagerOnly")]
    public class CoffeeShopManagerController : Controller
    {
        private readonly Context _context;

        public CoffeeShopManagerController(Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var managerName = User?.Identity?.Name ?? "Manager";
            ViewBag.ManagerName = managerName;

            // Dashboard Statistics
            var dashboardData = await GetDashboardDataAsync();
            return View(dashboardData);
        }

        public async Task<IActionResult> ProductAnalytics()
        {
            var productAnalytics = await GetProductAnalyticsAsync();
            return View(productAnalytics);
        }

        public async Task<IActionResult> SalesReport()
        {
            var salesData = await GetSalesReportAsync();
            return View(salesData);
        }

        public async Task<IActionResult> InventoryReport()
        {
            var inventoryData = await GetInventoryReportAsync();
            return View(inventoryData);
        }

        private async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            var totalProducts = await _context.Products.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalResources = await _context.Resources.CountAsync();

            var todayTransactions = await _context.Transactions
                .Where(t => t.Date.Date == today)
                .CountAsync();

            var todayRevenue = await _context.Transactions
                .Where(t => t.Date.Date == today)
                .SumAsync(t => (decimal?)t.TotalAmount) ?? 0;

            var thisMonthRevenue = await _context.Transactions
                .Where(t => t.Date >= thisMonth)
                .SumAsync(t => (decimal?)t.TotalAmount) ?? 0;

            var lastMonthRevenue = await _context.Transactions
                .Where(t => t.Date >= lastMonth && t.Date < thisMonth)
                .SumAsync(t => (decimal?)t.TotalAmount) ?? 0;

            var revenueGrowth = lastMonthRevenue > 0 
                ? ((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue) * 100 
                : 0;

            // Top selling products
            var topProducts = await _context.TransactionItems
                .Include(ti => ti.Product)
                .GroupBy(ti => new { ti.ProductId, ProductName = ti.Product!.Name })
                .Select(g => new TopProductViewModel
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(ti => ti.Quantity),
                    TotalRevenue = g.Sum(ti => ti.Quantity * ti.UnitPrice)
                })
                .OrderByDescending(p => p.TotalQuantity)
                .Take(5)
                .ToListAsync();

            return new DashboardViewModel
            {
                TotalProducts = totalProducts,
                TotalCategories = totalCategories,
                TotalResources = totalResources,
                TodayTransactions = todayTransactions,
                TodayRevenue = todayRevenue,
                ThisMonthRevenue = thisMonthRevenue,
                RevenueGrowth = revenueGrowth,
                TopProducts = topProducts
            };
        }

        private async Task<ProductAnalyticsViewModel> GetProductAnalyticsAsync()
        {
            var productsByCategory = await _context.Products
                .GroupBy(p => p.CategoryId)
                .Select(g => new CategoryProductCount
                {
                    CategoryId = g.Key,
                    CategoryName = _context.Categories.FirstOrDefault(c => c.Id == g.Key).Name ?? "Unknown",
                    ProductCount = g.Count()
                })
                .ToListAsync();

            var productSales = await _context.TransactionItems
                .Include(ti => ti.Product)
                .GroupBy(ti => new { ti.ProductId, ProductName = ti.Product!.Name })
                .Select(g => new ProductSalesData
                {
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(ti => ti.Quantity),
                    TotalRevenue = g.Sum(ti => ti.Quantity * ti.UnitPrice),
                    AveragePrice = g.Average(ti => ti.UnitPrice)
                })
                .OrderByDescending(p => p.TotalRevenue)
                .ToListAsync();

            return new ProductAnalyticsViewModel
            {
                ProductsByCategory = productsByCategory,
                ProductSales = productSales
            };
        }

        private async Task<SalesReportViewModel> GetSalesReportAsync()
        {
            var last30Days = DateTime.Today.AddDays(-30);
            
            var dailySales = await _context.Transactions
                .Where(t => t.Date >= last30Days)
                .GroupBy(t => t.Date.Date)
                .Select(g => new DailySalesData
                {
                    Date = g.Key,
                    TransactionCount = g.Count(),
                    TotalRevenue = g.Sum(t => t.TotalAmount)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            var monthlySales = await _context.Transactions
                .GroupBy(t => new { t.Date.Year, t.Date.Month })
                .Select(g => new MonthlySalesData
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TransactionCount = g.Count(),
                    TotalRevenue = g.Sum(t => t.TotalAmount)
                })
                .OrderBy(m => m.Year).ThenBy(m => m.Month)
                .ToListAsync();

            return new SalesReportViewModel
            {
                DailySales = dailySales,
                MonthlySales = monthlySales
            };
        }

        private async Task<InventoryReportViewModel> GetInventoryReportAsync()
        {
            var resources = await _context.Resources
                .Select(r => new ResourceInventoryData
                {
                    Name = r.Name,
                    Quantity = r.Quantity,
                    Unit = r.Unit,
                    CategoryName = r.Category != null ? r.Category.Name : "No Category"
                })
                .ToListAsync();

            var lowStockResources = resources.Where(r => r.Quantity < 10).ToList();

            return new InventoryReportViewModel
            {
                Resources = resources,
                LowStockResources = lowStockResources
            };
        }
    }


}
