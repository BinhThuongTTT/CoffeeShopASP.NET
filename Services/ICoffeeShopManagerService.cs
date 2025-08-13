using Microsoft.AspNetCore.Identity;

namespace CoffeeShopWeb.Services
{
    public interface ICoffeeShopManagerService
    {
        Task<List<IdentityUser>> GetManagersAsync(string search);
        Task<IdentityResult> AddManagerAsync(IdentityUser user, string password);
        Task DeleteManagerAsync(string id);
        Task<IdentityResult> ResetPasswordAsync(string id, string newPassword);
    }
}
