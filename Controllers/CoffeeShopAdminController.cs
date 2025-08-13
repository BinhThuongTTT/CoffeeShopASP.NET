using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = "AdminOnly")]
public class CoffeeShopAdminController : Controller
{
    public IActionResult Index()
    {
        var adminName = User?.Identity?.Name ?? "Admin";
        ViewBag.AdminName = adminName;
        return View();
    }
}
