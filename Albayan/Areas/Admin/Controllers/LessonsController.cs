using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Albayan.Services;
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
        private readonly IFileService _fileService;
        public LessonsController(PlatformDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
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
        [HttpGet]
        public async Task<IActionResult> GradeSubmission(int submissionId)
        {
            var submission = await _context.HomeworkSubmissions
                .Include(s => s.Student)
                .FirstOrDefaultAsync(s => s.Id == submissionId);

            if (submission == null) return NotFound();

            var viewModel = new GradeSubmissionViewModel
            {
                SubmissionId = submission.Id,
                StudentName = submission.Student.FullName,
                SubmissionFilePath = submission.SubmissionFilePath,
                Grade = submission.Grade,
                Feedback = submission.Feedback
            };

            return PartialView("_GradeSubmissionPartial", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> GradeSubmission(GradeSubmissionViewModel viewModel)
        {
            ModelState.Remove("StudentName");
            ModelState.Remove("SubmissionFilePath");
            if (ModelState.IsValid)
            {
                var submission = await _context.HomeworkSubmissions.FindAsync(viewModel.SubmissionId);
                if (submission != null)
                {
                    submission.Grade = viewModel.Grade;
                    submission.Feedback = viewModel.Feedback;
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "تم تقييم الواجب بنجاح!";
                    return Json(new { success = true });
                }
                return Json(new { success = false, errors = "Submission not found." });
            }

            var errors = ModelState.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).FirstOrDefault());
            return Json(new { success = false, errors = errors });
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lesson = await _context.Lessons
                .Include(l => l.Attachments)
                .Include(l => l.HomeworkSubmissions)
                    .ThenInclude(s => s.Student)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null) return NotFound();

            var viewModel = new LessonDetailsViewModel
            {
                Lesson = lesson,
                Submissions = lesson.HomeworkSubmissions.ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAttachment(int lessonId, IFormFile NewAttachment)
        {
            if (NewAttachment != null && NewAttachment.Length > 0)
            {
                var filePath = await _fileService.SaveFileAsync(NewAttachment, "attachments");
                var attachment = new LessonAttachment
                {
                    LessonId = lessonId,
                    FileName = Path.GetFileName(NewAttachment.FileName),
                    FilePath = filePath
                };
                _context.LessonAttachments.Add(attachment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", new { id = lessonId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            var attachment = await _context.LessonAttachments.FindAsync(attachmentId);
            if (attachment != null)
            {
                _fileService.DeleteFile(attachment.FilePath);
                _context.LessonAttachments.Remove(attachment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = attachment.LessonId });
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateHomework(int lessonId, string homeworkTitle, string homeworkDescription)
        {
            var lesson = await _context.Lessons.FindAsync(lessonId);
            if (lesson != null)
            {
                lesson.HomeworkTitle = homeworkTitle;
                lesson.HomeworkDescription = homeworkDescription;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تم حفظ الواجب بنجاح!";
            }
            return RedirectToAction("Details", new { id = lessonId });
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
            ModelState.Remove("Lesson.Attachments");
            ModelState.Remove("Lesson.HomeworkTitle");
            ModelState.Remove("Lesson.HomeworkDescription");
            ModelState.Remove("Lesson.HomeworkSubmissions");
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
            ModelState.Remove("Lesson.Attachments");
            ModelState.Remove("Lesson.HomeworkTitle");
            ModelState.Remove("Lesson.HomeworkDescription");
            ModelState.Remove("Lesson.HomeworkSubmissions");
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
