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
}
