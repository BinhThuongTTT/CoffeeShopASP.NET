using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoffeeShopWeb.Models
{
    public class Resource
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; }

        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
