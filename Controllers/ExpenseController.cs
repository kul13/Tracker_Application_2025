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
            return Ok(expenses);
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
                Category = dtos.Category,
                Notes = dtos.Notes,
                UserId = userId
            };
            await _service.AddAsync(expense);
            return Ok("Expense Added");
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] ExpenseCreateDto dtos)
        {
            var expense = new Expense
            {
                Amount = dtos.Amount,
                Date = dtos.Date,
                Category = dtos.Category,
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

            if (dtos == null || !dtos.Any())
                return BadRequest("Expense list cannot be empty");

            var expenses = dtos.Select(dto => new Expense
            {
                Amount = dto.Amount,
                Date = dto.Date,
                Category = dto.Category,
                Notes = dto.Notes
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
                         filteringDTOs.Category,
                         filteringDTOs.Month,
                         filteringDTOs.Year
            );
            var response = expenses.Select(e => new ExpenseCreateDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Category = e.Category,
                Notes = e.Notes
            });

            return Ok(response);
        }

    }
}
