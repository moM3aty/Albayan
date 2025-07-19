using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Exam
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public int PassPercentage { get; set; } 

        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
    }
}
