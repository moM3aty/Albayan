using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class GradeDetailsViewModel
    {
        public Grade Grade { get; set; }
        public IEnumerable<Course> Courses { get; set; }
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<EducationalMaterial> EducationalMaterials { get; set; }
        public IEnumerable<LiveLesson> LiveLessons { get; set; }
    }
}

