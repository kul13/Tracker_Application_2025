using ExpenseTracker.Api.Models;
using ExpenseTracker.Api.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ExpenseTracker.Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly ILogger<ExpenseController> _logger;
        private readonly IExpenseService _service;

        public ExpenseController(ILogger<ExpenseController> logger, IExpenseService expenseService)
        {
            _logger = logger;
            _service = expenseService;

        }
        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            var expenses = await _service.GetAllAsync();
            var response = expenses.Select(e => new ExpenseCreateDto
            {
                Amount = e.Amount,
                Date = e.Date,
                Notes = e.Notes,
                CategoryId = e.CategoryId,
                ItemId = e.ItemId,
                CategoryName = e.Category!.Name,
                ItemName = e.Item!.Name
            });

            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] ExpenseCreateDto dtos)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                         ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            var expense = new Expense
            {
                Amount = dtos.Amount,
                Date = dtos.Date,
                CategoryId = dtos.CategoryId,   // updated
                Notes = dtos.Notes,
                UserId = userId
            };
            await _service.AddAsync(expense, dtos.ItemName);
            return Ok("Expense Added");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseCreateDto dtos)
        {
            var expense = new Expense
            {
                Id = id,
                Amount = dtos.Amount,
                Date = dtos.Date,
                CategoryId = dtos.CategoryId,
                Notes = dtos.Notes
            };
            var isUpdated = await _service.UpdateAsync(id, expense);
            if (!isUpdated)
            {
                return NotFound("Expense Not Found");
            }
            return Ok("Expense Updated");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var isDeleted = await _service.DeleteAsync(id);
            if (!isDeleted)
            {
                return NotFound("Expense Not Found");
            }
            return Ok("Expense Deleted");
        }
        [HttpPost("bulk")]
        public async Task<IActionResult> AddBulkExpenses([FromBody] List<ExpenseCreateDto> dtos)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);

            if (dtos == null || !dtos.Any())
                return BadRequest("Expense list cannot be empty");

            var expenses = dtos.Select(dto => new Expense
            {
                Amount = dto.Amount,
                Date = dto.Date,
                CategoryId = dto.CategoryId,
                ItemId = dto.ItemId,
                Notes = dto.Notes,
                UserId = userId,
                TotalAmount = dto.TotalAmount

            }).ToList();

            await _service.AddBulkAsync(expenses);

            return Ok(new
            {
                Count = expenses.Count,
                Message = "Expenses added successfully"
            });
        }
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredExpenses([FromQuery] FilteringDTOs filteringDTOs)
        {
            var expenses = await _service.GetFilteredAsync(
                         filteringDTOs.FromDate,
                         filteringDTOs.ToDate,
                         filteringDTOs.CategoryId,
                         filteringDTOs.Month,
                         filteringDTOs.Year
            );
            var response = expenses.Select(e => new ExpenseCreateDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                CategoryId = e.CategoryId,
                Notes = e.Notes
            });

            return Ok(response);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _service.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("items/{categoryId}")]
        public async Task<IActionResult> GetItemsByCategoryId(int categoryId)
        {
            var items = await _service.GetByCategoryIdAsync(categoryId);
            return Ok(items);
        }
        [HttpGet("userexpenses")]
        public async Task<IActionResult> GetAllExpensesByUserId(int page = 1,int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                         ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (userIdClaim == null)
                return Unauthorized();
            int userId = int.Parse(userIdClaim.Value);
            var pagedExpenses = await _service.GetPagedByUserIdAsync(userId, page, pageSize);

            // Map the Items property of the paged result
            var response = pagedExpenses.Data.Select(e => new AuditExpenseDto
            {
                Id = e.Id,
                CategoryId = e.CategoryId,
                CategoryName = e.Category!.Name!,
                ItemId = (int)e.ItemId!,
                ItemName = e.Item?.Name ?? "N/A",
                Amount = e.Amount,
                Date= e.Date,
                Notes = e.Notes ?? ""
            }).ToList();

            return Ok(new
            {
                data = response,
                totalCount = pagedExpenses.TotalCount
            });

            
        }
    }
}
