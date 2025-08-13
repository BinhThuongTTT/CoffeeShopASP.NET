using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Policy = "Adminonly")]
    public class CoffeeShopStaffManagementController : Controller
    {
        private readonly ICoffeeShopStaffService _staffService;

        public CoffeeShopStaffManagementController(ICoffeeShopStaffService staffService)
        {
            _staffService = staffService;
        }

        public async Task<IActionResult> Index(string search)
        {
            var staff = await _staffService.GetStaffAsync(search);
            ViewBag.Search = search;
            return View(staff);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _staffService.AddStaffAsync(user, password);
            if (result.Succeeded)
            {
                await _staffService.AssignRoleAsync(user.Id, "Staff"); // <-- Gán role Staff
                TempData["Success"] = "Staff added";
                return RedirectToAction("Index");
            }
            foreach (var err in result.Errors)
            {
                ModelState.AddModelError("", err.Description);
            }
            return View();
        }

        // If someone navigates GET /Delete/id just redirect to Index (no immediate delete)
        public IActionResult Delete(string id)
        {
            return RedirectToAction("Index");
        }

        // Actual delete — POST with antiforgery
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _staffService.DeleteStaffAsync(id);
            TempData["Success"] = "Staff deleted";
            return RedirectToAction("Index");
        }

        // Show reset-password form
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = (await _staffService.GetStaffAsync(null)).FirstOrDefault(u => u.Id == id);
            if (user == null) return RedirectToAction("Index");
            return View(user);
        }

        // Handle reset-password POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword != confirmPassword)
            {
                TempData["Error"] = "Passwords are empty or do not match.";
                return RedirectToAction("ResetPassword", new { id });
            }

            var result = await _staffService.ResetPasswordAsync(id, newPassword);
            if (result.Succeeded)
            {
                TempData["Success"] = "Password reset";
                return RedirectToAction("Index");
            }

            TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            return RedirectToAction("ResetPassword", new { id });
        }

        // Optional assign role endpoints left as-is (you said manager page elsewhere); they can stay or be removed.
        public async Task<IActionResult> AssignRole(string id)
        {
            var user = await _staffService.GetStaffAsync(null).ContinueWith(t => t.Result.FirstOrDefault(u => u.Id == id));
            ViewBag.Roles = new SelectList(new[] { "Staff", "Manager" });
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string id, string role)
        {
            await _staffService.AssignRoleAsync(id, role);
            TempData["Success"] = "Role assigned";
            return RedirectToAction("Index");
        }
    }
}
