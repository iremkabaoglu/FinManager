using System.Collections.Generic;

namespace FinManager.Models
{
    public class DashboardViewModel
    {
        public decimal Income30 { get; set; }
        public decimal Expense30 { get; set; }
        public decimal Net30 => Income30 - Expense30;

        // Daily balance (last 30 days)
        public List<string> DailyLabels { get; set; } = new();
        public List<decimal> DailySeries { get; set; } = new();

        // Expenses by Category (last 30 days)
        public List<string> CategoryLabels { get; set; } = new();
        public List<decimal> CategorySeries { get; set; } = new();
    }
}
