using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class CoffeeShopManagerManagementController : Controller
    {
        private readonly ICoffeeShopManagerService _managerService;

        public CoffeeShopManagerManagementController(ICoffeeShopManagerService managerService)
        {
            _managerService = managerService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var managers = await _managerService.GetManagersAsync(search);
            ViewBag.Search = search;
            return View(managers);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _managerService.AddManagerAsync(user, password);
            if (result.Succeeded)
            {
                TempData["Success"] = "Manager added";
                return RedirectToAction("Index");
            }
            foreach (var err in result.Errors)
                ModelState.AddModelError("", err.Description);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            await _managerService.DeleteManagerAsync(id);
            TempData["Success"] = "Manager deleted";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ResetPassword(string id)
        {
            var managers = await _managerService.GetManagersAsync(null);
            var user = managers.FirstOrDefault(u => u.Id == id);
            if (user == null) return RedirectToAction("Index");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords are empty or do not match.";
                return RedirectToAction("ResetPassword", new { id });
            }

            var result = await _managerService.ResetPasswordAsync(id, newPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password reset";
                return RedirectToAction("Index");
            }

            TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            return RedirectToAction("ResetPassword", new { id });
        }
    }
}
