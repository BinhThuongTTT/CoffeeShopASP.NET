using CoffeeShopWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Services
{
    public class CoffeeShopStaffService : ICoffeeShopStaffService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Context _context;

        public CoffeeShopStaffService(UserManager<IdentityUser> userManager, Context context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<List<IdentityUser>> GetStaffAsync(string search)
        {
            var staffInRole = await (from user in _userManager.Users
                                     join userRole in _context.UserRoles on user.Id equals userRole.UserId
                                     join role in _context.Roles on userRole.RoleId equals role.Id
                                     where role.Name == "Staff"
                                     select user)
                            .ToListAsync();

            if (!string.IsNullOrEmpty(search))
            {
                staffInRole = staffInRole
                    .Where(u => u.Email.Contains(search) || u.Id.Contains(search))
                    .ToList();
            }

            return staffInRole;
        }

        public async Task<IdentityResult> AddStaffAsync(IdentityUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task DeleteStaffAsync(string id)
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

        public async Task AssignRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
                await _userManager.AddToRoleAsync(user, role);
        }
    }
}