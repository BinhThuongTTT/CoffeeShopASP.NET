using Microsoft.AspNetCore.Identity;

namespace CoffeeShopWeb.Services
{
    public interface ICoffeeShopStaffService
    {
        Task<List<IdentityUser>> GetStaffAsync(string search);
        Task<IdentityResult> AddStaffAsync(IdentityUser user, string password);
        Task DeleteStaffAsync(string id);
        Task<IdentityResult> ResetPasswordAsync(string id, string newPassword); // <-- changed
        Task AssignRoleAsync(string userId, string role);
    }
}