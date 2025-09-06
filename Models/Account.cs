using System.ComponentModel.DataAnnotations;

namespace FinManager.Models
{
    public class Account
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = default!;

        [StringLength(250)]
        public string? Description { get; set; }

        [DataType(DataType.Currency)]
        public decimal Balance { get; set; } = 0;  // Başlangıç bakiyesi

        // Identity User bağlantısı
        public string? OwnerId { get; set; }
    }
}
