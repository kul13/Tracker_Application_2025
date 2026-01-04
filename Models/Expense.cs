using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Notes { get; set; }

        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }     // Foreign Key

        //Navigation Property
        public User? User { get; set; }

        // New for cascading dropdown
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? ItemId { get; set; }
        public Item? Item { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string ?Name { get; set; }

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }

    public class Item
    {
        public int Id { get; set; }
        public string ?Name { get; set; }

        public int CategoryId { get; set; }   // Foreign Key
        public Category ?Category { get; set; }  // Navigation property
    }



}
