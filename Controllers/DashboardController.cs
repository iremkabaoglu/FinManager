using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FinManager.Data;
using FinManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinManager.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        // Category.Type alanın enum mu string mi?
        //  - Enum (CategoryType) ise true bırak
        //  - String ise false yap
        private const bool USE_ENUM = true;

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var end = DateTime.Today;
            var start = end.AddDays(-29); // son 30 gün (30 nokta)

            // Kullanıcının son 30 günlük işlemleri
            var tx = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.OwnerId == uid && t.Date.Date >= start && t.Date.Date <= end)
                .ToListAsync();

            // ----- Income / Expense toplamları -----
            Func<Transaction, bool> isIncome = t =>
                t.Category != null && (USE_ENUM
                    ? t.Category!.Type.Equals(CategoryType.Income)
                    : string.Equals((t.Category!.GetType().GetProperty("Type")!.GetValue(t.Category) ?? "").ToString(), "Income", StringComparison.OrdinalIgnoreCase));

            Func<Transaction, bool> isExpense = t =>
                t.Category != null && (USE_ENUM
                    ? t.Category!.Type.Equals(CategoryType.Expense)
                    : string.Equals((t.Category!.GetType().GetProperty("Type")!.GetValue(t.Category) ?? "").ToString(), "Expense", StringComparison.OrdinalIgnoreCase));

            decimal income = tx.Where(isIncome).Sum(t => t.Amount);
            decimal expense = tx.Where(isExpense).Sum(t => t.Amount);

            var vm = new DashboardViewModel
            {
                Income30 = income,
                Expense30 = expense
            };

            // ----- Günlük kümülatif net (bakiye eğrisi) -----
            var tr = new CultureInfo("tr-TR");
            decimal running = 0m;

            for (var d = start; d <= end; d = d.AddDays(1))
            {
                var dayIncome = tx.Where(t => t.Date.Date == d.Date && isIncome(t)).Sum(t => t.Amount);
                var dayExpense = tx.Where(t => t.Date.Date == d.Date && isExpense(t)).Sum(t => t.Amount);

                running += (dayIncome - dayExpense);

                vm.DailyLabels.Add(d.ToString("dd.MM", tr));
                vm.DailySeries.Add(running);
            }

            // ----- Kategoriye göre gider (donut) -----
            var byCat = tx.Where(isExpense)
                          .GroupBy(t => t.Category!.Name)
                          .Select(g => new { Name = g.Key, Total = g.Sum(x => x.Amount) })
                          .OrderByDescending(x => x.Total)
                          .ToList();

            foreach (var c in byCat)
            {
                vm.CategoryLabels.Add(c.Name);
                vm.CategorySeries.Add(c.Total);
            }

            return View(vm);
        }
    }
}
