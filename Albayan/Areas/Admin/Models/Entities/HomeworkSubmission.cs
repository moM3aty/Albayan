using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class HomeworkSubmission
    {
        public int Id { get; set; }
        [Required]
        public string SubmissionFilePath { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int? Grade { get; set; }
        public string? Feedback { get; set; }

        public int LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
