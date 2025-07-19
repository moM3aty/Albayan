using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LessonsController : Controller
    {
        private readonly PlatformDbContext _context;

        public LessonsController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Lessons/Index/5
        public async Task<IActionResult> Index(int? courseId)
        {
            if (courseId == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                return NotFound();
            }

            ViewBag.CourseTitle = course.Title;
            ViewBag.CourseId = course.Id;

            return View(course.Lessons.ToList());
        }

        // GET: Admin/Lessons/Create?courseId=5
        public async Task<IActionResult> Create(int? courseId)
        {
            if (courseId == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new LessonViewModel
            {
                Lesson = new Lesson { CourseId = course.Id },
                CourseId = course.Id,
                CourseTitle = course.Title
            };

            return View(viewModel);
        }

        // POST: Admin/Lessons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LessonViewModel viewModel)
        {
            ModelState.Remove("CourseTitle");
            ModelState.Remove("Lesson.Course");
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Lesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { courseId = viewModel.Lesson.CourseId });
            }
            return View(viewModel);
        }

        // GET: Admin/Lessons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            var course = await _context.Courses.FindAsync(lesson.CourseId);

            var viewModel = new LessonViewModel
            {
                Lesson = lesson,
                CourseId = course.Id,
                CourseTitle = course.Title
            };

            return View(viewModel);
        }

        // POST: Admin/Lessons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LessonViewModel viewModel)
        {
            if (id != viewModel.Lesson.Id) return NotFound();
            ModelState.Remove("CourseTitle");
            ModelState.Remove("Lesson.Course");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Lesson);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Lessons.Any(e => e.Id == viewModel.Lesson.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index), new { courseId = viewModel.Lesson.CourseId });
            }
            return View(viewModel);
        }

        // GET: Admin/Lessons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lesson == null) return NotFound();
            return View(lesson);
        }

        // POST: Admin/Lessons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            var courseId = lesson.CourseId;
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { courseId = courseId });
        }
    }
}
