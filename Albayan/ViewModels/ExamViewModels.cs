using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;

namespace Albayan.ViewModels
{
    // ViewModel to display the exam page to the student
    public class TakeExamViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public List<Question> Questions { get; set; }
        public Dictionary<int, string> StudentAnswers { get; set; } = new Dictionary<int, string>();

    }

    // ViewModel to display the result to the student
    public class ExamResultViewModel
    {
        public bool Passed { get; set; }
        public int Score { get; set; }
        public int PassPercentage { get; set; }
        public string CourseTitle { get; set; }
        public int? CertificateId { get; set; }
        public string CertificateGuid { get; set; } 
    }
}
