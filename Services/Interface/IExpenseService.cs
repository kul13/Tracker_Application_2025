using ExpenseTracker.Api.Models;

namespace ExpenseTracker.Api.Services.Interface
{
    public interface IExpenseService
    {
        Task<List<Expense>> GetAllAsync();
        Task AddAsync(Expense expense);
        Task<Expense?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, Expense expense);
        Task<bool> DeleteAsync(int id);
        Task AddBulkAsync(List<Expense> expenses);
        Task<List<Expense>> GetFilteredAsync( DateTime? fromDate,DateTime? toDate,string? category,int? month,int? year);

    }
}
