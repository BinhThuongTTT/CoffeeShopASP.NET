using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CoffeeShopWeb.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        // optional: staff who handled the transaction
        public string? StaffId { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public List<TransactionItem> Items { get; set; } = new();
        public DateTime TransactionDate { get; set; } = DateTime.Now;

    }
}
