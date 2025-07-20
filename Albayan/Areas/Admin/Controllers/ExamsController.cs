﻿using Albayan.Areas.Admin.Data;
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
    public class ExamsController : Controller
    {
        private readonly PlatformDbContext _context;

        public ExamsController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Exams/Index/5 (courseId)
        public async Task<IActionResult> Index(int? courseId)
        {
            if (courseId == null) return NotFound();

            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.CourseId == courseId);

            ViewBag.CourseTitle = course.Title;
            ViewBag.CourseId = course.Id;

            if (exam == null)
            {
                return View("NoExam", course);
            }

            var viewModel = new ExamViewModel
            {
                Exam = exam,
                CourseTitle = course.Title,
                Questions = exam.Questions
            };

            return View(viewModel);
        }

        // GET: Admin/Exams/Create?courseId=5
        public async Task<IActionResult> Create(int? courseId)
        {
            if (courseId == null) return NotFound();
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();

            var exam = new Exam
            {
                CourseId = course.Id,
                Title = $"اختبار دورة: {course.Title}"
            };

            ViewBag.CourseTitle = course.Title;
            return View(exam);
        }

        // POST: Admin/Exams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseId,Title,PassPercentage")] Exam exam)
        {
            ModelState.Remove("Course");
            ModelState.Remove("Questions");
            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { courseId = exam.CourseId });
            }
            var course = await _context.Courses.FindAsync(exam.CourseId);
            ViewBag.CourseTitle = course.Title;
            return View(exam);
        }

        // GET: Admin/Exams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return NotFound();

            var course = await _context.Courses.FindAsync(exam.CourseId);
            ViewBag.CourseTitle = course.Title;
            return View(exam);
        }

        // POST: Admin/Exams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CourseId,Title,PassPercentage")] Exam exam)
        {
            if (id != exam.CourseId)
            {
                return NotFound();
            }
            ModelState.Remove("Course");
            ModelState.Remove("Questions");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Exams.Any(e => e.CourseId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index), new { courseId = exam.CourseId });
            }
            var course = await _context.Courses.FindAsync(exam.CourseId);
            ViewBag.CourseTitle = course.Title;
            return View(exam);
        }

        // GET: Admin/Exams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (exam == null) return NotFound();
            return View(exam);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Courses");
        }
    }
}
