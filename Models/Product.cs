using System.ComponentModel.DataAnnotations;

namespace CoffeeShopWeb.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string? ImagePath { get; set; }
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; }
    }
}