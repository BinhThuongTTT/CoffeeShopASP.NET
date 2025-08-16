using CoffeeShopWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopWeb.Controllers
{
    [AllowAnonymous]
    public class CoffeeShopSetupController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CoffeeShopSetupController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdmin()
        {
            // Check if any admin already exists
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            if (adminUsers.Any())
            {
                TempData["Error"] = "Admin user đã tồn tại. Vui lòng đăng nhập.";
                return RedirectToAction("Login", "CoffeeShopAccount");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(string email, string password)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Email và mật khẩu không được để trống.";
                return View();
            }

            if (password.Length < 6)
            {
                TempData["Error"] = "Mật khẩu phải có ít nhất 6 ký tự.";
                return View();
            }

            // Check if any admin already exists
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            if (adminUsers.Any())
            {
                TempData["Error"] = "Admin user đã tồn tại. Không thể tạo thêm admin.";
                return RedirectToAction("Login", "CoffeeShopAccount");
            }

            // Check if user with this email already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                TempData["Error"] = "Email này đã được sử dụng. Vui lòng chọn email khác.";
                return View();
            }

            try
            {
                // Ensure Admin role exists
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Ensure other roles exist
                if (!await _roleManager.RoleExistsAsync("Manager"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Manager"));
                }

                if (!await _roleManager.RoleExistsAsync("Staff"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Staff"));
                }

                // Create user
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true // Auto-confirm for admin
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Add to Admin role
                    await _userManager.AddToRoleAsync(user, "Admin");

                    TempData["Success"] = "Admin user đã được tạo thành công! Bạn có thể đăng nhập ngay.";
                    return RedirectToAction("Login", "CoffeeShopAccount");
                }
                else
                {
                    // Collect all errors
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["Error"] = $"Không thể tạo admin user: {errors}";
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Đã xảy ra lỗi: {ex.Message}";
                return View();
            }
        }
    }
}