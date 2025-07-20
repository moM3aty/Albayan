using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SalesOutletsController : Controller
    {
        private readonly PlatformDbContext _context;

        public SalesOutletsController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SalesOutlets
        public async Task<IActionResult> Index()
        {
            var outlets = await _context.SalesOutlets
                .OrderBy(o => o.Governorate)
                .ThenBy(o => o.BookstoreName)
                .ToListAsync();
            return View(outlets);
        }

        // GET: Admin/SalesOutlets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/SalesOutlets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Governorate,BookstoreName")] SalesOutlet salesOutlet)
        {
            ModelState.Remove("AvailableMaterials");
            if (ModelState.IsValid)
            {
                _context.Add(salesOutlet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(salesOutlet);
        }

        // GET: Admin/SalesOutlets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var salesOutlet = await _context.SalesOutlets.FindAsync(id);
            if (salesOutlet == null) return NotFound();
            return View(salesOutlet);
        }

        // POST: Admin/SalesOutlets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Governorate,BookstoreName")] SalesOutlet salesOutlet)
        {
            if (id != salesOutlet.Id) return NotFound();
            ModelState.Remove("AvailableMaterials");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(salesOutlet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.SalesOutlets.Any(e => e.Id == salesOutlet.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(salesOutlet);
        }

        // GET: Admin/SalesOutlets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var salesOutlet = await _context.SalesOutlets.FirstOrDefaultAsync(m => m.Id == id);
            if (salesOutlet == null) return NotFound();
            return View(salesOutlet);
        }

        // POST: Admin/SalesOutlets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var salesOutlet = await _context.SalesOutlets.FindAsync(id);
            _context.SalesOutlets.Remove(salesOutlet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
