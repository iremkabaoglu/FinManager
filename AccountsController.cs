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
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Accounts
        public async Task<IActionResult> Index()
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var list = await _context.Accounts
                                     .Where(a => a.OwnerId == uid)
                                     .OrderBy(a => a.Name)
                                     .ToListAsync();
            return View(list);
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var account = await _context.Accounts
                                        .FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == uid);
            if (account is null) return NotFound();

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Balance")] Account account)
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            if (!ModelState.IsValid) return View(account);

            account.OwnerId = uid;
            _context.Add(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var account = await _context.Accounts
                                        .FirstOrDefaultAsync(a => a.Id == id && a.OwnerId == uid);
            if (account is null) return NotFound();

            return View(account);
        }

        // POST: Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Balance")] Account account)
        {
            if (id != account.Id) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var existing = await _context.Accounts
                                         .FirstOrDefaultAsync(a => a.Id == id && a.OwnerId == uid);
            if (existing is null) return NotFound();

            if (!ModelState.IsValid) return View(account);

            // Sadece düzenlenebilir alanları güncelle
            existing.Name = account.Name;
            existing.Description = account.Description;
            existing.Balance = account.Balance;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id, uid)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var account = await _context.Accounts
                                        .FirstOrDefaultAsync(m => m.Id == id && m.OwnerId == uid);
            if (account is null) return NotFound();

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var account = await _context.Accounts
                                        .FirstOrDefaultAsync(a => a.Id == id && a.OwnerId == uid);
            if (account is not null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id, string ownerId)
        {
            return _context.Accounts.Any(e => e.Id == id && e.OwnerId == ownerId);
        }
    }
}
