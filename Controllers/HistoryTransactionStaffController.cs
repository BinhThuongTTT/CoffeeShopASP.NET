using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Roles = "Staff")]
    public class HistoryTransactionStaffController : Controller
    {
        private readonly ITransactionService _transactionService;

        public HistoryTransactionStaffController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // Hiện toàn bộ lịch sử thanh toán của staff hiện tại
        [Route("CoffeeShopStaff/Details")]
        public async Task<IActionResult> Details()
        {
            var staffId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(staffId))
                return Unauthorized();

            var transactions = await _transactionService.GetTransactionsAsync(null, null, staffId);

            // Chỉ định đường dẫn view chính xác
            return View("~/Views/CoffeeShopStaff/Details.cshtml", transactions);
        }
    }
}
