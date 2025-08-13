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

        public CoffeeShopSetupController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Success"] = "Admin user created";
                return RedirectToAction("Login", "CoffeeShopAccount");
            }
            TempData["Error"] = "Failed to create Admin user";
            return View();
        }
    }
}