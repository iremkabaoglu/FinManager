using System.Linq;
using System.Threading.Tasks;
using FinManager.Data;
using FinManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FinManager.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TransactionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var list = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .Where(t => t.OwnerId == uid)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return View(list);
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var tx = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == uid);

            if (tx is null) return NotFound();
            return View(tx);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.OwnerId == uid), "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.OwnerId == uid), "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,CategoryId,Amount,Date,Note")] Transaction t)
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            // Hesap/kategori gerçekten bu kullanıcıya mı ait?
            bool ok = await _context.Accounts.AnyAsync(a => a.Id == t.AccountId && a.OwnerId == uid)
                   && await _context.Categories.AnyAsync(c => c.Id == t.CategoryId && c.OwnerId == uid);
            if (!ok) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.OwnerId == uid), "Id", "Name", t.AccountId);
                ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.OwnerId == uid), "Id", "Name", t.CategoryId);
                return View(t);
            }

            t.OwnerId = uid;
            _context.Add(t);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var tx = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == uid);
            if (tx is null) return NotFound();

            ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.OwnerId == uid), "Id", "Name", tx.AccountId);
            ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.OwnerId == uid), "Id", "Name", tx.CategoryId);
            return View(tx);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AccountId,CategoryId,Amount,Date,Note")] Transaction t)
        {
            if (id != t.Id) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var existing = await _context.Transactions
                .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == uid);
            if (existing is null) return NotFound();

            // Hesap/kategori sahipliği
            bool ok = await _context.Accounts.AnyAsync(a => a.Id == t.AccountId && a.OwnerId == uid)
                   && await _context.Categories.AnyAsync(c => c.Id == t.CategoryId && c.OwnerId == uid);
            if (!ok) return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["AccountId"] = new SelectList(_context.Accounts.Where(a => a.OwnerId == uid), "Id", "Name", t.AccountId);
                ViewData["CategoryId"] = new SelectList(_context.Categories.Where(c => c.OwnerId == uid), "Id", "Name", t.CategoryId);
                return View(t);
            }

            // Güncellenecek alanlar
            existing.AccountId = t.AccountId;
            existing.CategoryId = t.CategoryId;
            existing.Amount = t.Amount;
            existing.Date = t.Date;
            existing.Note = t.Note;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var tx = await _context.Transactions
                .Include(t => t.Account)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == uid);

            if (tx is null) return NotFound();
            return View(tx);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var tx = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == uid);
            if (tx is not null)
            {
                _context.Transactions.Remove(tx);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
