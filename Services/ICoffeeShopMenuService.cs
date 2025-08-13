using CoffeeShopWeb.Models;

namespace CoffeeShopWeb.Services
{
    public interface ICoffeeShopMenuService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product> GetProductAsync(int id);
        Task AddProductAsync(Product product, IFormFile image);
        Task UpdateProductAsync(Product product, IFormFile image);
        Task DeleteProductAsync(int id);
        Task<List<Category>> GetCategoriesAsync();
        Task AddCategoryAsync(Category category);
        Task DeleteCategoryAsync(int id);
        Task ResetProductIdSequenceAsync();
    }
}