using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Albayan.Models; 

namespace Albayan.Areas.Admin.Models.Entities
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        public DateTime RegistrationDate { get; set; }

        public int GradeId { get; set; }
        [NotMapped]
        public bool IsActive { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public string ApplicationUserId { get; set; }

        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Certificate> Certificates { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; }
        public virtual ICollection<TeacherRating> GivenRatings { get; set; }
    }
}