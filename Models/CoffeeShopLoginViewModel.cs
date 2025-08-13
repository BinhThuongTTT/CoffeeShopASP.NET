using System.ComponentModel.DataAnnotations;

namespace CoffeeShopWeb.Models
{
    public class CoffeeShopLoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}