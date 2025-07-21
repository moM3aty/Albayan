using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Question
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نص السؤال مطلوب")]
        [Display(Name = "نص السؤال")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "الخيار (أ) مطلوب")]
        [Display(Name = "الخيار أ")]
        public string OptionA { get; set; }

        [Required(ErrorMessage = "الخيار (ب) مطلوب")]
        [Display(Name = "الخيار ب")]
        public string OptionB { get; set; }

        [Display(Name = "الخيار ج")]
        public string? OptionC { get; set; }

        [Display(Name = "الخيار د")]
        public string? OptionD { get; set; }

        [Required(ErrorMessage = "يجب تحديد الإجابة الصحيحة")]
        [Display(Name = "الإجابة الصحيحة")]
        public string CorrectAnswer { get; set; }

        [Display(Name = "عدد الاختيارات")]
        [Range(2, 4)]
        public int NumberOfOptions { get; set; } = 4;

        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
    }
}
