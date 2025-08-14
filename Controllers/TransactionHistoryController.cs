using CoffeeShopWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopWeb.Controllers
{
    [Authorize(Roles = "Admin, Staff")]
    public class TransactionHistoryController : Controller
    {
        private readonly ITransactionService _transactionService;

        public TransactionHistoryController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<IActionResult> Index(DateTime? from, DateTime? to)
        {
            var transactions = await _transactionService.GetTransactionsAsync(from, to, null);
            return View(transactions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var tx = await _transactionService.GetTransactionByIdAsync(id);
            if (tx == null) return NotFound();
            return View(tx);
        }
    }
}
