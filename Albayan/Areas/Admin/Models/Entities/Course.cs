using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public double TotalHours { get; set; }

        // Foreign Keys
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الصف الدراسي")]
        [Display(Name = "الصف الدراسي")]
        public int GradeId { get; set; } // The new required property

        // Navigation Properties
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; } // The new navigation property

        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
        public virtual Exam Exam { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
    }
}
