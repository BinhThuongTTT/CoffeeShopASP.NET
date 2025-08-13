using CoffeeShopWeb.Models;

namespace CoffeeShopWeb.Services
{
    public interface IResourceService
    {
        Task<IEnumerable<Resource>> GetAllAsync(string search = null);
        Task<Resource?> GetByIdAsync(int id);
        Task AddAsync(Resource resource);
        Task UpdateAsync(Resource resource);
        Task DeleteAsync(int id);
        Task ResetResourceIdSequenceAsync();

    }
}
