using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Albayan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TeachersController : Controller
    {
        private readonly PlatformDbContext _context;
        private readonly IFileService _fileService;

        public TeachersController(PlatformDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        // GET: Admin/Teachers
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var teachersQuery = _context.Teachers
                .Select(t => new TeacherIndexViewModel
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    JobTitle = t.JobTitle,
                    ExperienceYears = t.ExperienceYears,
                    ProfileImageUrl = t.ProfileImageUrl
                });

            if (!String.IsNullOrEmpty(searchString))
            {
                teachersQuery = teachersQuery.Where(t => t.FullName.Contains(searchString) || t.JobTitle.Contains(searchString));
            }

            var teachers = await teachersQuery.OrderBy(t => t.FullName).ToListAsync();
            return View(teachers);
        }

        // GET: Admin/Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        // GET: Admin/Teachers/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new TeacherFormViewModel
            {
                Teacher = new Teacher(),
                Subjects = await _context.Subjects.Select(s => new AssignedSubjectViewModel
                {
                    SubjectId = s.Id,
                    Name = s.Name,
                    IsAssigned = false
                }).ToListAsync()
            };
            return View(viewModel);
        }

        // GET: Admin/Teachers/CreatePartial
        public IActionResult CreatePartial()
        {
            return PartialView("_CreatePartial", new Teacher());
        }

        // POST: Admin/Teachers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherFormViewModel viewModel)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            var teacher = viewModel.Teacher;

            if (isAjax)
            {
                ModelState.Remove("Teacher.Subjects");
                ModelState.Remove("ProfileImage");
            }
            ModelState.Remove("Teacher.Courses");
            ModelState.Remove("Teacher.Ratings");
            ModelState.Remove("Teacher.Subjects");
            ModelState.Remove("Teacher.LiveLessons");
            ModelState.Remove("Teacher.ProfileImageUrl");
            if (ModelState.IsValid)
            {
                if (!isAjax && viewModel.ProfileImage != null)
                {
                    teacher.ProfileImageUrl = await _fileService.SaveFileAsync(viewModel.ProfileImage, "teachers");
                }

                if (!isAjax && viewModel.Subjects != null)
                {
                    teacher.Subjects = new List<Subject>();
                    foreach (var subjectVM in viewModel.Subjects.Where(s => s.IsAssigned))
                    {
                        var subject = await _context.Subjects.FindAsync(subjectVM.SubjectId);
                        if (subject != null) teacher.Subjects.Add(subject);
                    }
                }

                _context.Add(teacher);
                await _context.SaveChangesAsync();

                if (isAjax) return Json(new { success = true, id = teacher.Id, text = teacher.FullName });

                return RedirectToAction(nameof(Index));
            }

            if (isAjax)
            {
                var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault());
                return Json(new { success = false, errors = errors });
            }

            var allSubjects = await _context.Subjects.ToListAsync();
            var postedSubjectIds = new HashSet<int>(viewModel.Subjects?.Where(s => s.IsAssigned).Select(s => s.SubjectId) ?? Enumerable.Empty<int>());
            viewModel.Subjects = allSubjects.Select(s => new AssignedSubjectViewModel
            {
                SubjectId = s.Id,
                Name = s.Name,
                IsAssigned = postedSubjectIds.Contains(s.Id)
            }).ToList();
            return View(viewModel);
        }

        // GET: Admin/Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound();

            var allSubjects = await _context.Subjects.ToListAsync();
            var viewModel = new TeacherFormViewModel
            {
                Teacher = teacher,
                Subjects = allSubjects.Select(s => new AssignedSubjectViewModel
                {
                    SubjectId = s.Id,
                    Name = s.Name,
                    IsAssigned = teacher.Subjects.Any(ts => ts.Id == s.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/Teachers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TeacherFormViewModel viewModel)
        {
            if (id != viewModel.Teacher.Id) return NotFound();

            ModelState.Remove("Teacher.Courses");
            ModelState.Remove("ProfileImage");
            ModelState.Remove("Teacher.Ratings");
            ModelState.Remove("Teacher.Subjects");
            ModelState.Remove("Teacher.LiveLessons");
            ModelState.Remove("Teacher.ProfileImageUrl");
            if (ModelState.IsValid)
            {
                var teacherToUpdate = await _context.Teachers
                    .Include(t => t.Subjects)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (teacherToUpdate == null) return NotFound();

                if (viewModel.ProfileImage != null)
                {
                    _fileService.DeleteFile(teacherToUpdate.ProfileImageUrl);
                    teacherToUpdate.ProfileImageUrl = await _fileService.SaveFileAsync(viewModel.ProfileImage, "teachers");
                }

                teacherToUpdate.FullName = viewModel.Teacher.FullName;
                teacherToUpdate.JobTitle = viewModel.Teacher.JobTitle;
                teacherToUpdate.ExperienceSummary = viewModel.Teacher.ExperienceSummary;
                teacherToUpdate.ExperienceYears = viewModel.Teacher.ExperienceYears;

                teacherToUpdate.Subjects.Clear();
                if (viewModel.Subjects != null)
                {
                    foreach (var subjectVM in viewModel.Subjects.Where(s => s.IsAssigned))
                    {
                        var subject = await _context.Subjects.FindAsync(subjectVM.SubjectId);
                        if (subject != null) teacherToUpdate.Subjects.Add(subject);
                    }
                }

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Teachers.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var allSubjects = await _context.Subjects.ToListAsync();
            var postedSubjectIds = new HashSet<int>(viewModel.Subjects?.Where(s => s.IsAssigned).Select(s => s.SubjectId) ?? Enumerable.Empty<int>());
            viewModel.Subjects = allSubjects.Select(s => new AssignedSubjectViewModel
            {
                SubjectId = s.Id,
                Name = s.Name,
                IsAssigned = postedSubjectIds.Contains(s.Id)
            }).ToList();
            return View(viewModel);
        }

        // GET: Admin/Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        // POST: Admin/Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                _fileService.DeleteFile(teacher.ProfileImageUrl);
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
