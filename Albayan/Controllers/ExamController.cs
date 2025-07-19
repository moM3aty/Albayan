using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.ViewModels;
using System;
using System.Collections.Generic;

namespace Albayan.Controllers
{
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
                return NotFound("لا يوجد اختبار لهذه الدورة.");
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
            if (viewModel == null)
            {
                return BadRequest("البيانات المرسلة غير صالحة.");
            }

            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CourseId == viewModel.CourseId);

            if (exam == null) return NotFound("لم يتم العثور على الاختبار.");
            if (exam.Course == null) return NotFound("لم يتم العثور على الدورة المرتبطة بهذا الاختبار.");
            if (exam.Questions == null) return NotFound("لم يتم تحميل أسئلة هذا الاختبار.");

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
            int? newCertificateId = null;
            string newCertificateGuid = null;

            if (passed)
            {
                try
                {
                    int currentStudentId = 1; 

                    var existingCert = await _context.Certificates
                        .FirstOrDefaultAsync(c => c.StudentId == currentStudentId && c.CourseId == exam.CourseId);

                    if (existingCert == null)
                    {
                        var certificate = new Certificate
                        {
                            StudentId = currentStudentId,
                            CourseId = exam.CourseId,
                            IssueDate = DateTime.UtcNow,
                            GradePercentage = score,
                            CertificateGuid = Guid.NewGuid().ToString()
                        };
                        _context.Certificates.Add(certificate);
                        await _context.SaveChangesAsync();
                        newCertificateId = certificate.Id;
                        newCertificateGuid = certificate.CertificateGuid;
                    }
                    else
                    {
                        newCertificateId = existingCert.Id;
                        newCertificateGuid = existingCert.CertificateGuid;
                    }
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "حدث خطأ أثناء إنشاء الشهادة. " + ex.Message);
                }
            }

            var resultViewModel = new ExamResultViewModel
            {
                Passed = passed,
                Score = score,
                PassPercentage = exam.PassPercentage,
                CourseTitle = exam.Course.Title,
                CertificateId = newCertificateId,
                 CertificateGuid = newCertificateGuid
            };

            return View("Result", resultViewModel);
        }
    }
}
