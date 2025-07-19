using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Albayan.Areas.Admin.Data;
using Albayan.ViewModels;

namespace Albayan.Controllers
{
    public class CoursesController : Controller
    {
        private readonly PlatformDbContext _context;

        public CoursesController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: /Courses
        public async Task<IActionResult> Index(int? gradeId, int? subjectId, string searchTerm)
        {
            var coursesQuery = _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Subject)
                .Include(c => c.Lessons)
                .AsQueryable();

            // Apply filters
            if (gradeId.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.GradeId == gradeId.Value);
            }
            if (subjectId.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.SubjectId == subjectId.Value);
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                coursesQuery = coursesQuery.Where(c => c.Title.Contains(searchTerm) || c.Teacher.FullName.Contains(searchTerm));
            }

            var courses = await coursesQuery
                .Select(c => new CourseCardViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    CoverImageUrl = c.CoverImageUrl,
                    TeacherName = c.Teacher.FullName,
                    TotalHours = c.TotalHours,
                    LessonsCount = c.Lessons.Count,
                    AverageRating = 4.8 
                }).ToListAsync();

            var viewModel = new PublicCoursesViewModel
            {
                Courses = courses,
                Grades = new SelectList(await _context.Grades.ToListAsync(), "Id", "Name", gradeId),
                Subjects = new SelectList(await _context.Subjects.ToListAsync(), "Id", "Name", subjectId),
                SelectedGradeId = gradeId,
                SelectedSubjectId = subjectId,
                SearchTerm = searchTerm
            };

            return View(viewModel);
        }
    }
}
