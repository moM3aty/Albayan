using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class LessonMaterial
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان المادة مطلوب")]
        [Display(Name = "العنوان")]
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

        [Required(ErrorMessage = "يجب اختيار المادة")]
        [Display(Name = "المادة")]
        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }
}
