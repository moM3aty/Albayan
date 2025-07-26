using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Teacher")]
    public class DashboardController : Controller
    {
        private readonly PlatformDbContext _context;

        public DashboardController(PlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalTeachers = await _context.Teachers.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();

            var studentData = await _context.Students
                .Where(s => s.RegistrationDate >= DateTime.Now.AddMonths(-6))
                .GroupBy(s => new { s.RegistrationDate.Year, s.RegistrationDate.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            var chartLabels = new List<string>();
            var chartData = new List<int>();
            for (int i = 5; i >= 0; i--)
            {
                var date = DateTime.Now.AddMonths(-i);
                var monthData = studentData.FirstOrDefault(d => d.Year == date.Year && d.Month == date.Month);
                chartLabels.Add(new CultureInfo("ar-EG").DateTimeFormat.GetMonthName(date.Month));
                chartData.Add(monthData?.Count ?? 0);
            }

            var latestStudents = await _context.Students
                .OrderByDescending(s => s.RegistrationDate)
                .Take(5)
                .Select(s => new StudentIndexViewModel
                {
                    FullName = s.FullName,
                    RegistrationDate = s.RegistrationDate
                })
                .ToListAsync();

            var popularCourses = await _context.Courses
                .OrderByDescending(c => c.StudentCourses.Count())
                .Take(5)
                .Select(c => new CourseIndexViewModel
                {
                    Title = c.Title,
                    LessonsCount = c.StudentCourses.Count()
                })
                .ToListAsync();

            var latestPosts = await _context.BlogPosts
                .OrderByDescending(p => p.PublishDate)
                .Take(5)
                .Select(p => new BlogPostIndexViewModel
                {
                    Title = p.Title,
                    PublishDate = p.PublishDate
                })
                .ToListAsync();

            var recentSubmissions = await _context.HomeworkSubmissions
                .Include(s => s.Student)
                .Include(s => s.Lesson)
                    .ThenInclude(l => l.Course)
                .OrderByDescending(s => s.SubmissionDate)
                .Take(5)
                .Select(s => new HomeworkSubmissionViewModel
                {
                    StudentName = s.Student != null ? s.Student.FullName : "طالب غير معروف",
                    LessonTitle = s.Lesson != null ? s.Lesson.Title : "درس غير معروف",
                    CourseTitle = s.Lesson != null && s.Lesson.Course != null ? s.Lesson.Course.Title : "دورة غير معروفة",
                    SubmissionDate = s.SubmissionDate
                })
                .ToListAsync();

            var viewModel = new DashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalCourses = totalCourses,
                MonthlyRevenue = 15780, 
                NewStudentsChartLabels = chartLabels,
                NewStudentsChartData = chartData,
                LatestStudents = latestStudents,
                PopularCourses = popularCourses,
                LatestBlogPosts = latestPosts,
                RecentSubmissions = recentSubmissions
            };

            return View(viewModel);
        }
    }
}
