using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Stage
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المرحلة مطلوب")]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Grade> Grades { get; set; }
    }
}