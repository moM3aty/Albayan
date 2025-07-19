using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CertificatesController : Controller
    {
        private readonly PlatformDbContext _context;

        public CertificatesController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Certificates
        public async Task<IActionResult> Index()
        {
            var certificates = await _context.Certificates
                .Include(c => c.Student)
                .Include(c => c.Course)
                .Select(c => new CertificateIndexViewModel
                {
                    Id = c.Id,
                    StudentName = c.Student.FullName,
                    CourseTitle = c.Course.Title,
                    IssueDate = c.IssueDate,
                    GradePercentage = c.GradePercentage
                })
                .OrderByDescending(c => c.IssueDate)
                .ToListAsync();

            return View(certificates);
        }

        // GET: Admin/Certificates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var certificate = await _context.Certificates
                .Include(c => c.Student)
                .Include(c => c.Course)
                .ThenInclude(co => co.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null) return NotFound();
            return View(certificate);
        }

        // GET: Admin/Certificates/Create
        public IActionResult Create()
        {
            var viewModel = new CertificateFormViewModel
            {
                Certificate = new Certificate { IssueDate = DateTime.Now, CertificateGuid = Guid.NewGuid().ToString() },
                Students = new SelectList(_context.Students, "Id", "FullName"),
                Courses = new SelectList(_context.Courses, "Id", "Title")
            };
            return View(viewModel);
        }

        // POST: Admin/Certificates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificateFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Certificate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.Students = new SelectList(_context.Students, "Id", "FullName", viewModel.Certificate.StudentId);
            viewModel.Courses = new SelectList(_context.Courses, "Id", "Title", viewModel.Certificate.CourseId);
            return View(viewModel);
        }

        // GET: Admin/Certificates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();

            var viewModel = new CertificateFormViewModel
            {
                Certificate = certificate,
                Students = new SelectList(_context.Students, "Id", "FullName", certificate.StudentId),
                Courses = new SelectList(_context.Courses, "Id", "Title", certificate.CourseId)
            };
            return View(viewModel);
        }

        // POST: Admin/Certificates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CertificateFormViewModel viewModel)
        {
            if (id != viewModel.Certificate.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Certificate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Certificates.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            viewModel.Students = new SelectList(_context.Students, "Id", "FullName", viewModel.Certificate.StudentId);
            viewModel.Courses = new SelectList(_context.Courses, "Id", "Title", viewModel.Certificate.CourseId);
            return View(viewModel);
        }

        // GET: Admin/Certificates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var certificate = await _context.Certificates
                .Include(c => c.Student)
                .Include(c => c.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (certificate == null) return NotFound();
            return View(certificate);
        }

        // POST: Admin/Certificates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
