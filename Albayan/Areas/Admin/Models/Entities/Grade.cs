using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Grade
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الصف مطلوب")]
        [StringLength(100)]
        public string Name { get; set; }

        public int StageId { get; set; }

        [ForeignKey("StageId")]
        public virtual Stage Stage { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<EducationalMaterial> EducationalMaterials { get; set; }

    }
}
