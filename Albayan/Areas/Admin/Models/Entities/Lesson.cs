using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الدرس مطلوب")]
        [StringLength(200)]
        [Display(Name = "عنوان الدرس")]
        public string Title { get; set; }

        [Display(Name = "معرف مكتبة الفيديو (Bunny.net)")]
        public string BunnyVideoLibraryId { get; set; }

        [Display(Name = "معرف الفيديو (Bunny.net)")]
        public string BunnyVideoId { get; set; }

        public int CourseId { get; set; }

        [Display(Name = "عنوان الواجب")]
        public string? HomeworkTitle { get; set; }

        [Display(Name = "وصف الواجب")]
        public string? HomeworkDescription { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public virtual ICollection<LessonAttachment> Attachments { get; set; }
        public virtual ICollection<HomeworkSubmission> HomeworkSubmissions { get; set; }
    }
}
