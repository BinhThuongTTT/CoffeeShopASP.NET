using CoffeeShopWeb.Models;
using Microsoft.AspNetCore.Http;

namespace CoffeeShopWeb.Services
{
    public interface IStaffBillingService
    {
        Task<List<BillingItem>> GetCartAsync(ISession session);
        Task AddToCartAsync(ISession session, BillingItem item);
        Task UpdateCartItemAsync(ISession session, int productId, int newQty);
        Task RemoveFromCartAsync(ISession session, int productId);
        Task ClearCartAsync(ISession session);
        decimal CalculateTotal(IEnumerable<BillingItem> items);
    }
}
