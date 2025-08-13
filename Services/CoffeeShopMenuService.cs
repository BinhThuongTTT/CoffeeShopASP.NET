using CoffeeShopWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace CoffeeShopWeb.Services
{
    public class CoffeeShopMenuService : ICoffeeShopMenuService
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IMemoryCache _memoryCache;

        public CoffeeShopMenuService(Context context, IWebHostEnvironment environment, IMemoryCache memoryCache)
        {
            _context = context;
            _environment = environment;
            _memoryCache = memoryCache;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var cacheKey = "products";
            if (!_memoryCache.TryGetValue(cacheKey, out List<Product> products))
            {
                products = await _context.Products.ToListAsync();
                _memoryCache.Set(cacheKey, products, TimeSpan.FromMinutes(10));
            }
            return products;
        }

        public async Task AddProductAsync(Product product, IFormFile image)
        {
            if (image != null)
            {
                var path = Path.Combine(_environment.WebRootPath, "images", image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    await image.CopyToAsync(stream);
                product.ImagePath = $"/images/{image.FileName}";
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            _memoryCache.Remove("products");
        }

        public async Task<Product> GetProductAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task UpdateProductAsync(Product product, IFormFile image)
        {
            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.IsAvailable = product.IsAvailable;

                var path = Path.Combine(_environment.WebRootPath, "images", image.FileName);
                using (var stream = new FileStream(path, FileMode.Create))
                    await image.CopyToAsync(stream);
                existingProduct.ImagePath = $"/images/{image.FileName}";

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();
                _memoryCache.Remove("products");
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _memoryCache.Remove("products");
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }


        public async Task AddCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetProductIdSequenceAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Products', RESEED, 0)");
        }
    }
}