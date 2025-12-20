using ExpenseTracker.Api.DTOs;
using ExpenseTracker.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseTracker.Api.Services.Interface
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDTO dto);
    }
}
