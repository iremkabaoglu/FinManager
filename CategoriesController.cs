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
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoriesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var list = await _context.Categories
                                     .Where(c => c.OwnerId == uid)
                                     .OrderBy(c => c.Name)
                                     .ToListAsync();
            return View(list);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == uid);
            if (category is null) return NotFound();

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create() => View();

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Type")] Category category)
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            if (!ModelState.IsValid) return View(category);

            category.OwnerId = uid;
            _context.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == uid);
            if (category is null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type")] Category category)
        {
            if (id != category.Id) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var existing = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == uid);
            if (existing is null) return NotFound();

            if (!ModelState.IsValid) return View(category);

            existing.Name = category.Name;
            existing.Type = category.Type;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return NotFound();

            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == uid);
            if (category is null) return NotFound();

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uid = _userManager.GetUserId(User);
            if (uid is null) return Challenge();

            var category = await _context.Categories
                                         .FirstOrDefaultAsync(c => c.Id == id && c.OwnerId == uid);
            if (category is not null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
