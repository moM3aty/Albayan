using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;

namespace Albayan.ViewModels
{
    public class TakeLessonQuizViewModel
    {
        public LessonQuiz Quiz { get; set; }
        public Dictionary<int, string> Answers { get; set; }
    }
}