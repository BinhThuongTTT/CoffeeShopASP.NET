using CoffeeShopWeb.Models;

namespace CoffeeShopWeb.Services
{
    public interface ITransactionService
    {
        Task<int> CreateTransactionAsync(string? staffId, List<BillingItem> items);
        Task<IEnumerable<Transaction>> GetTransactionsAsync(DateTime? from = null, DateTime? to = null, string? staffId = null);
        Task<Transaction?> GetTransactionByIdAsync(int id);
    }
}
