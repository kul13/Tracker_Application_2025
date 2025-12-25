using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.DTOs
{
    public class ExpenseCreateDto
    {
        public int Id { get; internal set; }
        [Required]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }


        [MaxLength(200)]
        public string? Notes { get; set; }

        public int CategoryId { get; set; }
        public int? ItemId { get; set; }

        public string? CategoryName { get; set; }
        public string? ItemName { get; set; }

    }
}
