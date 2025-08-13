using CoffeeShopWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Services
{
    public class CoffeeShopManagerService : ICoffeeShopManagerService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Context _context;

        public CoffeeShopManagerService(UserManager<IdentityUser> userManager, Context context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<IdentityUser>> GetManagersAsync(string search)
        {
            var query = from user in _userManager.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == "Manager"
                        select user;

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.Email.Contains(search) || u.Id.Contains(search));
            }

            return await query.ToListAsync();
        }

        public async Task<IdentityResult> AddManagerAsync(IdentityUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Manager");
            }
            return result;
        }

        public async Task DeleteManagerAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(string id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result;
        }
    }
}