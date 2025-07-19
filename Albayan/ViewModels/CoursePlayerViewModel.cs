using System.Collections.Generic;
using Albayan.Areas.Admin.Models.Entities;

namespace Albayan.ViewModels
{
    public class CoursePlayerViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public Lesson CurrentLesson { get; set; }
        public List<Lesson> AllLessons { get; set; }
        public int? NextLessonId { get; set; }
        public int? PreviousLessonId { get; set; }
        public int StudentProgress { get; set; } 
    }
}
