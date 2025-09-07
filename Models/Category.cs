namespace FinManager.Models
{
    public enum CategoryType
    {
        Income = 0,
        Expense = 1
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public CategoryType Type { get; set; }   // <— enum
        public string? OwnerId { get; set; }
    }
}
