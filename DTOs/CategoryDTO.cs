namespace ExpenseTracker.Api.DTOs
{
    public class CategoryDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<ItemDto> Items { get; set; } = new List<ItemDto>();
    }
    public class ItemDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
