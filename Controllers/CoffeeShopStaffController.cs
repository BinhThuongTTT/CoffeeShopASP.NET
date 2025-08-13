using CoffeeShopWeb.Models;
using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Roles = "Staff")]
    public class CoffeeShopStaffController : Controller
    {
        private readonly ICoffeeShopMenuService _menuService;
        private readonly IStaffBillingService _billingService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<CoffeeShopStaffController> _logger;

        public CoffeeShopStaffController(
            ICoffeeShopMenuService menuService,
            IStaffBillingService billingService,
            ITransactionService transactionService,
            ILogger<CoffeeShopStaffController> logger)
        {
            _menuService = menuService;
            _billingService = billingService;
            _transactionService = transactionService;
            _logger = logger;
        }

        // GET: /CoffeeShopStaff
        public async Task<IActionResult> Index()
        {
            var products = await _menuService.GetProductsAsync(); // assume existing method
            var cart = await _billingService.GetCartAsync(HttpContext.Session);
            ViewBag.Cart = cart;
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            // fetch product details (name, price)
            var product = await _menuService.GetProductAsync(productId);
            if (product == null) return NotFound();

            var item = new BillingItem
            {
                ProductId = productId,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };

            await _billingService.AddToCartAsync(HttpContext.Session, item);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartItem(int productId, int quantity)
        {
            await _billingService.UpdateCartItemAsync(HttpContext.Session, productId, quantity);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCartItem(int productId)
        {
            await _billingService.RemoveFromCartAsync(HttpContext.Session, productId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            var cart = await _billingService.GetCartAsync(HttpContext.Session);
            var vm = new BillViewModel
            {
                Items = cart,
                Date = DateTime.UtcNow,
                StaffId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmPayment()
        {
            var cart = await _billingService.GetCartAsync(HttpContext.Session);
            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] = "Cart is empty.";
                return RedirectToAction(nameof(Index));
            }

            var staffId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var txId = await _transactionService.CreateTransactionAsync(staffId, cart);
            await _billingService.ClearCartAsync(HttpContext.Session);

            return RedirectToAction(nameof(Bill), new { id = txId });
        }

        public async Task<IActionResult> Bill(int id)
        {
            var tx = await _transactionService.GetTransactionByIdAsync(id);
            if (tx == null) return NotFound();

            var vm = new BillViewModel
            {
                TransactionId = tx.Id,
                Date = tx.Date,
                StaffId = tx.StaffId,
                Items = tx.Items.Select(i => new BillingItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    Name = _menuService.GetProductAsync(i.ProductId).Result?.Name // small sync; acceptable here for simplicity
                }).ToList()
            };

            return View(vm);
        }
    }
}
