using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class ExamViewModel
    {
        public Exam Exam { get; set; }
        public string CourseTitle { get; set; }
        public ICollection<Question> Questions { get; set; }
    }

}
