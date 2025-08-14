using CoffeeShopWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly Context _context;

        public TransactionService(Context context)
        {
            _context = context;
        }

        public async Task<int> CreateTransactionAsync(string? staffId, List<BillingItem> items)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("No items to create transaction.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var transaction = new Transaction
                {
                    Date = DateTime.UtcNow,
                    StaffId = staffId,
                    TotalAmount = items.Sum(i => i.LineTotal)
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                var tItems = items.Select(i => new TransactionItem
                {
                    TransactionId = transaction.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price,
                    UnitPrice = i.Price * i.Quantity
                }).ToList();

                _context.TransactionItems.AddRange(tItems);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return transaction.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(DateTime? from = null, DateTime? to = null, string? staffId = null)
        {
            var q = _context.Transactions
                .Include(t => t.Items)
                    .ThenInclude(i => i.Product) // lấy luôn thông tin sản phẩm
                .AsQueryable();

            if (from.HasValue)
                q = q.Where(t => t.Date >= from.Value);

            if (to.HasValue)
                q = q.Where(t => t.Date <= to.Value);

            if (!string.IsNullOrEmpty(staffId))
                q = q.Where(t => t.StaffId == staffId);

            return await q.OrderByDescending(t => t.Date).ToListAsync();
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Items)
                    .ThenInclude(i => i.Product) // lấy tên sản phẩm
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
