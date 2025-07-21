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
        public int NumberOfOptions { get; set; }
        public List<SelectListItem> CorrectAnswerOptions { get; set; }

        public QuestionFormViewModel()
        {
            CorrectAnswerOptions = new List<SelectListItem>();
        }

        public void GenerateCorrectAnswerOptions()
        {
            var options = new List<SelectListItem>
            {
                new SelectListItem { Value = "أ", Text = "الخيار أ" },
                new SelectListItem { Value = "ب", Text = "الخيار ب" }
            };

            if (NumberOfOptions >= 3)
            {
                options.Add(new SelectListItem { Value = "ج", Text = "الخيار ج" });
            }
            if (NumberOfOptions >= 4)
            {
                options.Add(new SelectListItem { Value = "د", Text = "الخيار د" });
            }
            CorrectAnswerOptions = options;
        }
    }
}