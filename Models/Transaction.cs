using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation; 

namespace FinManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required, Display(Name = "Account")]
        public int AccountId { get; set; }

        [ValidateNever]                 
        public Account? Account { get; set; }   

        [Required, Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ValidateNever]                 
        public Category? Category { get; set; } 

        [Required, DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [StringLength(250)]
        public string? Note { get; set; }

        public string? OwnerId { get; set; }
    }
}
