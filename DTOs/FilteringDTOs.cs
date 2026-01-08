namespace ExpenseTracker.Api.DTOs
{
    public class FilteringDTOs
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? CategoryId { get; set; }
        public int? Month { get; set; }  
        public int? Year { get; set; }
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public List<T> Data { get; set; } = [];
    }

    public  class AuditExpenseDto
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
