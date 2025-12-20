using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Models
{
    public class Expense
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }


        public int UserId { get; set; }     // Foreign Key

        //Navigation Property
        public User? User { get; set; }
    }
}
