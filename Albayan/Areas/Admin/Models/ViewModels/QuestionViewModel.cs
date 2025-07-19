using Albayan.Areas.Admin.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class QuestionFormViewModel
    {
        public Question Question { get; set; }
        public int ExamId { get; set; } 
        public string CourseTitle { get; set; }
        public List<SelectListItem> CorrectAnswerOptions { get; set; }

        public QuestionFormViewModel()
        {
            CorrectAnswerOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "A", Text = "الخيار أ" },
                new SelectListItem { Value = "B", Text = "الخيار ب" },
                new SelectListItem { Value = "C", Text = "الخيار ج" },
                new SelectListItem { Value = "D", Text = "الخيار د" }
            };
        }
    }
}
