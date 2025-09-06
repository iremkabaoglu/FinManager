using System.ComponentModel.DataAnnotations;

namespace FinManager.Models
{
    public enum CategoryType
    {
        Income = 1,
        Expense = 2
    }

    public class Category
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        [Required]
        public CategoryType Type { get; set; }

        public string? OwnerId { get; set; }
    }
}
