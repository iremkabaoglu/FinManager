using FinManager.Data;
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

        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var from = DateTime.Today.AddDays(-30);
            var tx = await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.OwnerId == uid && t.Date >= from)
                .ToListAsync();

            var income = tx.Where(t => t.Category.Type == Models.CategoryType.Income).Sum(t => t.Amount);
            var expense = tx.Where(t => t.Category.Type == Models.CategoryType.Expense).Sum(t => t.Amount);
            var net = income - expense;

            var daily = tx.GroupBy(t => t.Date.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(x => x.Amount * (x.Category.Type == Models.CategoryType.Expense ? -1 : 1)) })
                .OrderBy(g => g.Date)
                .ToList();

            var expenseByCat = tx.Where(t => t.Category.Type == Models.CategoryType.Expense)
                .GroupBy(t => t.Category.Name)
                .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                .ToList();

            ViewBag.Income = income;
            ViewBag.Expense = expense;
            ViewBag.Net = net;

            ViewBag.DailyLabels = daily.Select(d => d.Date.ToString("dd.MM")).ToArray();
            ViewBag.DailyTotals = daily.Select(d => d.Total).ToArray();

            ViewBag.PieLabels = expenseByCat.Select(e => e.Category).ToArray();
            ViewBag.PieTotals = expenseByCat.Select(e => e.Total).ToArray();

            return View();

        }

    }
}
