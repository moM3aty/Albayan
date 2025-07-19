using System.Collections.Generic;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public decimal MonthlyRevenue { get; set; } 

        public List<string> NewStudentsChartLabels { get; set; }
        public List<int> NewStudentsChartData { get; set; }
    }
}
