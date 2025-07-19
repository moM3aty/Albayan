using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Albayan.Areas.Admin.Data;
using Albayan.ViewModels;

namespace Albayan.Controllers
{
    public class CoursePlayerController : Controller
    {
        private readonly PlatformDbContext _context;

        public CoursePlayerController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: /CoursePlayer/Index/{courseId}/{lessonId?}
        public async Task<IActionResult> Index(int courseId, int? lessonId)
        {
            var course = await _context.Courses
                .Include(c => c.Lessons)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null || !course.Lessons.Any())
            {
                return NotFound("لم يتم العثور على الدورة أو لا تحتوي على دروس.");
            }

            var lessons = course.Lessons.OrderBy(l => l.Id).ToList();
            var currentLesson = lessonId.HasValue
                ? lessons.FirstOrDefault(l => l.Id == lessonId.Value)
                : lessons.FirstOrDefault();

            if (currentLesson == null)
            {
                return NotFound("لم يتم العثور على الدرس المطلوب.");
            }

            var currentLessonIndex = lessons.FindIndex(l => l.Id == currentLesson.Id);

            var viewModel = new CoursePlayerViewModel
            {
                CourseId = course.Id,
                CourseTitle = course.Title,
                AllLessons = lessons,
                CurrentLesson = currentLesson,
                PreviousLessonId = currentLessonIndex > 0 ? lessons[currentLessonIndex - 1].Id : (int?)null,
                NextLessonId = currentLessonIndex < lessons.Count - 1 ? lessons[currentLessonIndex + 1].Id : (int?)null,
                StudentProgress = (int)System.Math.Round(((double)(currentLessonIndex + 1) / lessons.Count) * 100)
            };

            return View(viewModel);
        }
    }
}
