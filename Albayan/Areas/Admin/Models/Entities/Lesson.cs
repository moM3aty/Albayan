using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Lesson
    {
        public int Id { get; set; }
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        [Display(Name = "رابط الفيديو (Embed URL)")]
        public string VideoUrl { get; set; }
        public int CourseId { get; set; }

        public string? HomeworkTitle { get; set; }
        public string? HomeworkDescription { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public virtual ICollection<LessonAttachment> Attachments { get; set; }
        public virtual ICollection<HomeworkSubmission> HomeworkSubmissions { get; set; }
    }
}