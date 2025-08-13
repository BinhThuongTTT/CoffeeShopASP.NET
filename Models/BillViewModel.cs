using System;
using System.Collections.Generic;

namespace CoffeeShopWeb.Models
{
    public class BillViewModel
    {
        public int? TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string? StaffId { get; set; }
        public List<BillingItem> Items { get; set; } = new();
        public decimal Total => Items?.Sum(i => i.LineTotal) ?? 0m;
    }
}
