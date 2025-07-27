using Albayan.Areas.Admin.Models.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class LessonDetailsViewModel
    {
        public Lesson Lesson { get; set; }

        [Display(Name = "ملف مرفق جديد")]
        public IFormFile NewAttachment { get; set; }
    }
}
