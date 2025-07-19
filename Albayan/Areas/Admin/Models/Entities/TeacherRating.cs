using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class TeacherRating
    {
        public int Id { get; set; }

        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون بين 1 و 5 نجوم.")]
        public int Rating { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }

        public DateTime RatingDate { get; set; }
        public int TeacherId { get; set; }
        public int StudentId { get; set; } 

        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
    }
}
