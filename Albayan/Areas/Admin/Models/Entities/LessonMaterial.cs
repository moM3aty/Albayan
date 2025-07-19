using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class LessonMaterial
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الدرس مطلوب")]
        [Display(Name = "عنوان الدرس")]
        public string Title { get; set; }

        [Display(Name = "الوصف")]
        public string Description { get; set; }

        [Display(Name = "صورة الغلاف")]
        public string CoverImageUrl { get; set; }

        [Display(Name = "ملف PDF")]
        public string PdfFilePath { get; set; }

        [Display(Name = "عدد التحميلات")]
        public int Downloads { get; set; }

        [Display(Name = "تاريخ الرفع")]
        public DateTime UploadDate { get; set; }

        // Foreign Key to Subject
        [Required(ErrorMessage = "يجب اختيار المادة")]
        [Display(Name = "المادة")]
        public int SubjectId { get; set; }

        // Navigation Property
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }
}
