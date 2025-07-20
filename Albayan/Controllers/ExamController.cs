using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Albayan.Controllers
{
    [Authorize(Roles = "Student")]
    public class ExamController : Controller
    {
        private readonly PlatformDbContext _context;

        public ExamController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: /Exam/Take/5 (courseId)
        public async Task<IActionResult> Take(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            var exam = await _context.Exams
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CourseId == id);

            if (exam == null)
            {
                return View("NoExamAvailable", course);
            }

            var questions = await _context.Questions
                .Where(q => q.ExamId == exam.CourseId)
                .ToListAsync();

            var viewModel = new TakeExamViewModel
            {
                CourseId = exam.CourseId,
                CourseTitle = course.Title,
                Questions = questions
            };

            return View(viewModel);
        }

        // POST: /Exam/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(TakeExamViewModel viewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.ApplicationUserId == userId);
            if (student == null) return Unauthorized();

            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CourseId == viewModel.CourseId);

            if (exam == null) return NotFound("لم يتم العثور على الاختبار.");

            int correctAnswers = 0;
            foreach (var question in exam.Questions)
            {
                if (viewModel.StudentAnswers != null && viewModel.StudentAnswers.ContainsKey(question.Id) &&
                    viewModel.StudentAnswers[question.Id] == question.CorrectAnswer)
                {
                    correctAnswers++;
                }
            }

            int totalQuestions = exam.Questions.Count;
            int score = totalQuestions > 0 ? (int)Math.Round((double)correctAnswers / totalQuestions * 100) : 0;
            bool passed = score >= exam.PassPercentage;
            string newCertificateGuid = null;

            if (passed)
            {
                var existingCert = await _context.Certificates
                    .FirstOrDefaultAsync(c => c.StudentId == student.Id && c.CourseId == exam.CourseId);

                if (existingCert == null)
                {
                    var certificate = new Certificate
                    {
                        StudentId = student.Id,
                        CourseId = exam.CourseId,
                        IssueDate = DateTime.UtcNow,
                        GradePercentage = score,
                        CertificateGuid = Guid.NewGuid().ToString()
                    };
                    _context.Certificates.Add(certificate);
                    await _context.SaveChangesAsync();
                    newCertificateGuid = certificate.CertificateGuid;
                }
                else
                {
                    newCertificateGuid = existingCert.CertificateGuid;
                }
            }

            var resultViewModel = new ExamResultViewModel
            {
                Passed = passed,
                Score = score,
                PassPercentage = exam.PassPercentage,
                CourseTitle = exam.Course.Title,
                CertificateGuid = newCertificateGuid
            };

            return View("Result", resultViewModel);
        }
    }
}
