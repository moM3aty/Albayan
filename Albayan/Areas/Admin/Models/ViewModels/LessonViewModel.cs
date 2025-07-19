using Albayan.Areas.Admin.Models.Entities;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class LessonViewModel
    {
        public Lesson Lesson { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
    }
}
