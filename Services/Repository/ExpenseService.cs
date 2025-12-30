using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Services.Interface;
using Microsoft.Identity.Client;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseTracker.Api.Services.Repository
{
    public class ExpenseService : IExpenseService 
    {
        public readonly AppDbContext _context;
      
        public ExpenseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Expense>> GetAllAsync()
        {
            return await _context.Expenses
            .Include(e => e.User)
            .Include(e => e.Category)
            .Include(e => e.Item)
            .ToListAsync();
        }
        public async Task AddAsync(Expense expense , string? ItemName)
        {
            if (!string.IsNullOrWhiteSpace(ItemName))
            {
                var item = await _context.Items.FirstOrDefaultAsync(i =>
                    i.CategoryId == expense.CategoryId &&
                    i.Name == ItemName);

                if (item == null)
                {
                    item = new Item
                    {
                        Name = ItemName,
                        CategoryId = expense.CategoryId
                    };

                    _context.Items.Add(item);
                    await _context.SaveChangesAsync();
                }

                expense.ItemId = item.Id;
            }

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            //await _context.Expenses.AddAsync(expense);
            //await _context.SaveChangesAsync();
        }
        public async Task<Expense?> GetByIdAsync(int id)
        {
            return await _context.Expenses
             .Include(e => e.Category)
             .Include(e => e.Item)
             .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<bool> UpdateAsync(int id, Expense expense)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null) return false;

            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.Notes = expense.Notes;
            existingExpense.CategoryId = expense.CategoryId;
            existingExpense.ItemId = expense.ItemId;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
            {
                return false;
            }
            _context.Expenses.Remove(existingExpense);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task AddBulkAsync(List<Expense> expenses)
        {
            await _context.Expenses.AddRangeAsync(expenses);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Expense>> GetFilteredAsync(DateTime? fromDate, DateTime? toDate, int? categoryId, int? month, int? year)
        {
            var query = _context.Expenses.AsQueryable();
            if (fromDate.HasValue)
            {
                query = query.Where(e => e.Date >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(e => e.Date <= toDate.Value);
            }
             if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId);
            }
            if (month.HasValue)
            {
                query = query.Where(e => e.Date.Month == month.Value);
            }
            if (year.HasValue)
            {
                query = query.Where(e => e.Date.Year == year.Value);
            }
            return await query.ToListAsync();
        }

        public async  Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories
            .Include(c => c.Items)
            .ToListAsync();

        }

        public async Task<List<Item>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Items
                .AsNoTracking()
                .Where(i => i.CategoryId == categoryId)
                .OrderBy(i => i.Name)
                .ToListAsync();
        }
    }
}
