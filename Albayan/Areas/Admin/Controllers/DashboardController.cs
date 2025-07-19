using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly PlatformDbContext _context;

        public DashboardController(PlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get stats
            var totalStudents = await _context.Students.CountAsync();
            var totalTeachers = await _context.Teachers.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();

            // Get chart data for new students in the last 6 months
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

            var viewModel = new DashboardViewModel
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalCourses = totalCourses,
                MonthlyRevenue = 15780,
                NewStudentsChartLabels = chartLabels,
                NewStudentsChartData = chartData
            };

            return View(viewModel);
        }
    }
}
