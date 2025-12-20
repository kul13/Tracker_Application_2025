using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.Services.Interface;
using Microsoft.Identity.Client;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

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
            return await _context.Expenses.ToListAsync();
        }
        public async Task AddAsync(Expense expense)
        {
            await _context.Expenses.AddAsync(expense);
            await _context.SaveChangesAsync();
        }
        public async Task<Expense?> GetByIdAsync(int id)
        {
            return await _context.Expenses.FindAsync(id);
        }
        public async Task<bool> UpdateAsync(int id, Expense expense)
        {
            var existingExpense = await _context.Expenses.FindAsync(id);
            if (existingExpense == null)
            {
                return false;
            }
            existingExpense.Amount = expense.Amount;
            existingExpense.Date = expense.Date;
            existingExpense.Category = expense.Category;
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
        public async Task<List<Expense>> GetFilteredAsync(DateTime? fromDate, DateTime? toDate, string? category, int? month, int? year)
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
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category == category);
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

    }
}
