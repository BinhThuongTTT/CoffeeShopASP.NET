using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CoffeeShopWeb.Models;

namespace CoffeeShopWeb.Controllers
{
    [Authorize]
    public class CoffeeShopAccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CoffeeShopAccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(CoffeeShopLoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var roles = await _userManager.GetRolesAsync(user);

                TempData["Success"] = "Login successful";

                if (roles.Contains("Admin"))
                    return RedirectToAction("Index", "CoffeeShopAdmin");
                else if (roles.Contains("Manager"))
                    return RedirectToAction("Index", "CoffeeShopManager");
                else if (roles.Contains("Staff"))
                    return RedirectToAction("Index", "CoffeeShopStaff");
                else
                    return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = "Invalid login attempt";
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                TempData["Success"] = "Password reset link sent";
            }
            return RedirectToAction("Login");
        }
    }
}