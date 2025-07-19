using Microsoft.AspNetCore.Http;
using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    // ViewModel for listing teachers
    public class TeacherIndexViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public int ExperienceYears { get; set; }
        public string ProfileImageUrl { get; set; }
    }

    // ViewModel for the Create/Edit form
    public class TeacherFormViewModel
    {
        public Teacher Teacher { get; set; }

        [Display(Name = "صورة الملف الشخصي")]
        public IFormFile ProfileImage { get; set; }

        [Display(Name = "المواد التي يدرسها")]
        public List<AssignedSubjectViewModel> Subjects { get; set; }
    }

    // ViewModel to represent a single subject with an assignment status
    public class AssignedSubjectViewModel
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public bool IsAssigned { get; set; }
    }
}
