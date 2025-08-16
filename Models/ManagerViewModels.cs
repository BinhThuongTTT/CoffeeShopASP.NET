namespace CoffeeShopWeb.Models
{
    // ViewModels for Manager Dashboard
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalResources { get; set; }
        public int TodayTransactions { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
        public List<TopProductViewModel> TopProducts { get; set; } = new();
    }

    public class TopProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ProductAnalyticsViewModel
    {
        public List<CategoryProductCount> ProductsByCategory { get; set; } = new();
        public List<ProductSalesData> ProductSales { get; set; } = new();
    }

    public class CategoryProductCount
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    public class ProductSalesData
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AveragePrice { get; set; }
    }

    public class SalesReportViewModel
    {
        public List<DailySalesData> DailySales { get; set; } = new();
        public List<MonthlySalesData> MonthlySales { get; set; } = new();
    }

    public class DailySalesData
    {
        public DateTime Date { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlySalesData
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class InventoryReportViewModel
    {
        public List<ResourceInventoryData> Resources { get; set; } = new();
        public List<ResourceInventoryData> LowStockResources { get; set; } = new();
    }

    public class ResourceInventoryData
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}
