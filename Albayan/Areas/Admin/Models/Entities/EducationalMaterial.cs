using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class EducationalMaterial
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الدرس مطلوب")]
        [Display(Name = "عنوان الدرس")]
        public string Title { get; set; }

        [Display(Name = "وصف الدرس")]
        public string Description { get; set; }

        [Display(Name = "صورة الغلاف")]
        public string CoverImageUrl { get; set; }

        [Display(Name = "التقييم (من 5)")]
        public double Rating { get; set; }

        [Display(Name = "عدد الصفحات")]
        public int PageCount { get; set; }

        // Foreign Keys
        [Required(ErrorMessage = "يجب اختيار الصف")]
        [Display(Name = "الصف الدراسي")]
        public int GradeId { get; set; }

        [Required(ErrorMessage = "يجب اختيار المادة")]
        [Display(Name = "المادة")]
        public int SubjectId { get; set; }

        // Navigation Properties
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        // Many-to-many relationship with SalesOutlet
        public virtual ICollection<SalesOutlet> SalesOutlets { get; set; }
    }
}