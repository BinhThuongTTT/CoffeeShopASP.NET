using CoffeeShopWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Services
{
    public class ResourceService : IResourceService
    {
        private readonly Context _context;

        public ResourceService(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Resource>> GetAllAsync(string search = null)
        {
            var query = _context.Resources.Include(r => r.Category).AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(r => r.Name.Contains(search));

            return await query.ToListAsync();
        }

        public async Task<Resource?> GetByIdAsync(int id)
        {
            return await _context.Resources.Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Resource resource)
        {
            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resource resource)
        {
            _context.Resources.Update(resource);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            if (resource != null)
            {
                _context.Resources.Remove(resource);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetResourceIdSequenceAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Resources', RESEED, 0)");
        }

    }
}
