using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopWeb.Models
{
    public class TransactionItem
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        [ForeignKey("TransactionId")]
        public Transaction? Transaction { get; set; }

        public int ProductId { get; set; }
        // optional navigation to Product
        // public Product? Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; } // unit price at time of sale
        public decimal UnitPrice { get; set; }
    }
}
