namespace ExpenseTracker.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string Email { get; set; }
        public string? PasswordHash { get; set; } // Store hashed password

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // 1 user → many expenses
        public ICollection<Expense> ?Expenses { get; set; }
    }

    public class UserRole
    {
        public int UserId { get; set; } //Foreign Key
        public int Id { get; set; }  // Primary Key
        public string RoleName { get; set; } = null!;

        // Navigation
        public User User { get; set; } = null!;
    }
}
