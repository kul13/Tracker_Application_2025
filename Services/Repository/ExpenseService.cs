using ExpenseTracker.Api.Data;
using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Services.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

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
            //if (!string.IsNullOrWhiteSpace(ItemName))
            //{
            //    var item = await _context.Items.FirstOrDefaultAsync(i =>
            //        i.CategoryId == expense.CategoryId &&
            //        i.Name == ItemName);

            //    if (item == null)
            //    {
            //        item = new Item
            //        {
            //            Name = ItemName,
            //            CategoryId = expense.CategoryId
            //        };

            //        _context.Items.Add(item);
            //        await _context.SaveChangesAsync();
            //    }

            //    expense.ItemId = item.Id;
            //}

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

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
            var itemIds = expenses
                .Select(e => e.ItemId)
                .Distinct()
                .ToList();

            var validItemIds = await _context.Items
                .Where(i => itemIds.Contains(i.Id))
                .Select(i => i.Id)
                .ToListAsync();

            if (validItemIds.Count != itemIds.Count)
                throw new ValidationException("One or more ItemIds are invalid");

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
               .Select(c => new Category
               {
                   Id = c.Id,
                   Name = c.Name
               }).ToListAsync();
        }

        public async Task<List<ItemDto>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Items
                .Where(i => i.CategoryId == categoryId)
                .Select(i => new ItemDto
                {
                    Id = i.Id,
                    Name = i.Name,
                    CategoryId = i.CategoryId
                })
                .ToListAsync();
        }
    }
}
