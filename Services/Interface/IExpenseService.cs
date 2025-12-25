using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Services.Interface
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllAsync();
        Task AddAsync(Expense expense, string? ItemName);
        Task<Expense?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Expense expense);
        Task<bool> DeleteAsync(int id);
        Task AddBulkAsync(List<Expense> expenses);
        Task<List<Expense>> GetFilteredAsync( DateTime? fromDate,DateTime? toDate,int? categoryId,int? month,int? year);

        Task<List<Category>> GetAllCategoriesAsync();

    }

   
   
}
