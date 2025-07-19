using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Albayan.Services;
using Microsoft.AspNetCore.Authorization;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LessonMaterialsController : Controller
    {
        private readonly PlatformDbContext _context;
        private readonly IFileService _fileService;

        public LessonMaterialsController(PlatformDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.LessonMaterials.Include(l => l.Subject).OrderByDescending(s => s.UploadDate).ToListAsync());
        }

        public IActionResult Create()
        {
            var viewModel = new LessonMaterialFormViewModel
            {
                LessonMaterial = new LessonMaterial { UploadDate = DateTime.Now },
                Subjects = new SelectList(_context.Subjects, "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonMaterialFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var material = viewModel.LessonMaterial;
                if (viewModel.CoverImage != null)
                {
                    material.CoverImageUrl = await _fileService.SaveFileAsync(viewModel.CoverImage, "materials_covers");
                }
                if (viewModel.PdfFile != null)
                {
                    material.PdfFilePath = await _fileService.SaveFileAsync(viewModel.PdfFile, "materials_pdfs");
                }
                _context.Add(material);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.Subjects = new SelectList(_context.Subjects, "Id", "Name", viewModel.LessonMaterial.SubjectId);
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var material = await _context.LessonMaterials.FindAsync(id);
            if (material == null) return NotFound();
            var viewModel = new LessonMaterialFormViewModel
            {
                LessonMaterial = material,
                Subjects = new SelectList(_context.Subjects, "Id", "Name", material.SubjectId)
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LessonMaterialFormViewModel viewModel)
        {
            if (id != viewModel.LessonMaterial.Id) return NotFound();

            ModelState.Remove("CoverImage");
            ModelState.Remove("PdfFile");

            if (ModelState.IsValid)
            {
                var materialToUpdate = await _context.LessonMaterials.FindAsync(id);
                if (materialToUpdate == null) return NotFound();

                if (viewModel.CoverImage != null)
                {
                    _fileService.DeleteFile(materialToUpdate.CoverImageUrl);
                    materialToUpdate.CoverImageUrl = await _fileService.SaveFileAsync(viewModel.CoverImage, "materials_covers");
                }
                if (viewModel.PdfFile != null)
                {
                    _fileService.DeleteFile(materialToUpdate.PdfFilePath);
                    materialToUpdate.PdfFilePath = await _fileService.SaveFileAsync(viewModel.PdfFile, "materials_pdfs");
                }

                materialToUpdate.Title = viewModel.LessonMaterial.Title;
                materialToUpdate.Description = viewModel.LessonMaterial.Description;
                materialToUpdate.SubjectId = viewModel.LessonMaterial.SubjectId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.Subjects = new SelectList(_context.Subjects, "Id", "Name", viewModel.LessonMaterial.SubjectId);
            return View(viewModel);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var material = await _context.LessonMaterials.Include(l => l.Subject).FirstOrDefaultAsync(m => m.Id == id);
            if (material == null) return NotFound();
            return View(material);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.LessonMaterials.FindAsync(id);
            if (material != null)
            {
                _fileService.DeleteFile(material.CoverImageUrl);
                _fileService.DeleteFile(material.PdfFilePath);
                _context.LessonMaterials.Remove(material);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}