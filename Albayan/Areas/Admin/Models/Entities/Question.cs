using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Question
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نص السؤال مطلوب")]
        public string QuestionText { get; set; }

        [Required(ErrorMessage = "الخيار (أ) مطلوب")]
        public string OptionA { get; set; }

        [Required(ErrorMessage = "الخيار (ب) مطلوب")]
        public string OptionB { get; set; }

        [Required(ErrorMessage = "الخيار (ج) مطلوب")]
        public string OptionC { get; set; }

        [Required(ErrorMessage = "الخيار (د) مطلوب")]
        public string OptionD { get; set; }

        [Required(ErrorMessage = "يجب تحديد الإجابة الصحيحة")]
        public string CorrectAnswer { get; set; }

        public int ExamId { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; }
    }
}
