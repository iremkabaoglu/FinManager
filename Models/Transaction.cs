using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinManager.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public int AccountId { get; set; }
        public Account Account { get; set; } = default!;

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        [Required, DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [StringLength(250)]
        public string? Note { get; set; }

        public string? OwnerId { get; set; }
    }
}
