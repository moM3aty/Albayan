using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class QuestionsController : Controller
    {
        private readonly PlatformDbContext _context;

        public QuestionsController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Questions/Create?examId=5
        public async Task<IActionResult> Create(int? examId)
        {
            if (examId == null) return NotFound();
            var exam = await _context.Exams.Include(e => e.Course).FirstOrDefaultAsync(e => e.CourseId == examId);
            if (exam == null) return NotFound();

            var viewModel = new QuestionFormViewModel
            {
                Question = new Question { ExamId = exam.CourseId },
                ExamId = exam.CourseId,
                CourseTitle = exam.Course.Title
            };

            return View(viewModel);
        }

        // POST: Admin/Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionFormViewModel viewModel)
        {
            ModelState.Remove("CourseTitle");
            ModelState.Remove("Question.Exam");
            if (ModelState.IsValid)
            {
                _context.Add(viewModel.Question);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Exams", new { courseId = viewModel.Question.ExamId });
            }
            var exam = await _context.Exams.Include(e => e.Course).FirstOrDefaultAsync(e => e.CourseId == viewModel.ExamId);
            if (exam != null)
            {
                viewModel.CourseTitle = exam.Course.Title;
            }
            viewModel.CorrectAnswerOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "A", Text = "الخيار أ" },
                new SelectListItem { Value = "B", Text = "الخيار ب" },
                new SelectListItem { Value = "C", Text = "الخيار ج" },
                new SelectListItem { Value = "D", Text = "الخيار د" }
            };

            return View(viewModel);
        }

        // GET: Admin/Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();

            var exam = await _context.Exams.Include(e => e.Course).FirstOrDefaultAsync(e => e.CourseId == question.ExamId);
            var viewModel = new QuestionFormViewModel
            {
                Question = question,
                ExamId = exam.CourseId,
                CourseTitle = exam.Course.Title
            };

            return View(viewModel);
        }

        // POST: Admin/Questions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuestionFormViewModel viewModel)
        {
            if (id != viewModel.Question.Id) return NotFound();
            ModelState.Remove("CourseTitle");
            ModelState.Remove("Question.Exam");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel.Question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Questions.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction("Index", "Exams", new { courseId = viewModel.Question.ExamId });
            }
            var exam = await _context.Exams.Include(e => e.Course).FirstOrDefaultAsync(e => e.CourseId == viewModel.Question.ExamId);
            if (exam != null)
            {
                viewModel.CourseTitle = exam.Course.Title;
            }

            viewModel.CorrectAnswerOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "A", Text = "الخيار أ" },
                new SelectListItem { Value = "B", Text = "الخيار ب" },
                new SelectListItem { Value = "C", Text = "الخيار ج" },
                new SelectListItem { Value = "D", Text ="الخيار د" }
            };
            return View(viewModel);
        }

        // GET: Admin/Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var question = await _context.Questions
                .Include(q => q.Exam)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null) return NotFound();
            return View(question);
        }

        // POST: Admin/Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            var examId = question.ExamId;
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Exams", new { courseId = examId });
        }
    }
}
