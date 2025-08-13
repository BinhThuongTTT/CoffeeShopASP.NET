using CoffeeShopWeb.Models;
using CoffeeShopWeb.Extensions;
using Microsoft.AspNetCore.Http;

namespace CoffeeShopWeb.Services
{
    public class StaffBillingService : IStaffBillingService
    {
        private const string SessionKey = "StaffCart_v1";

        public async Task<List<BillingItem>> GetCartAsync(ISession session)
        {
            // session is synchronous API; keep method async for consistency
            var cart = session.GetObject<List<BillingItem>>(SessionKey);
            return cart ?? new List<BillingItem>();
        }

        public Task AddToCartAsync(ISession session, BillingItem item)
        {
            var cart = session.GetObject<List<BillingItem>>(SessionKey) ?? new List<BillingItem>();
            var existing = cart.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existing != null)
            {
                existing.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
            session.SetObject(SessionKey, cart);
            return Task.CompletedTask;
        }

        public Task UpdateCartItemAsync(ISession session, int productId, int newQty)
        {
            var cart = session.GetObject<List<BillingItem>>(SessionKey) ?? new List<BillingItem>();
            var existing = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
            {
                if (newQty <= 0) cart.Remove(existing);
                else existing.Quantity = newQty;
            }
            session.SetObject(SessionKey, cart);
            return Task.CompletedTask;
        }

        public Task RemoveFromCartAsync(ISession session, int productId)
        {
            var cart = session.GetObject<List<BillingItem>>(SessionKey) ?? new List<BillingItem>();
            var existing = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null) cart.Remove(existing);
            session.SetObject(SessionKey, cart);
            return Task.CompletedTask;
        }

        public Task ClearCartAsync(ISession session)
        {
            session.Remove(SessionKey);
            return Task.CompletedTask;
        }

        public decimal CalculateTotal(IEnumerable<BillingItem> items)
        {
            return items?.Sum(i => i.LineTotal) ?? 0m;
        }
    }
}
