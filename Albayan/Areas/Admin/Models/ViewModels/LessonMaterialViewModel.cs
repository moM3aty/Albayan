using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Albayan.Areas.Admin.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class LessonMaterialFormViewModel
    {
        public LessonMaterial LessonMaterial { get; set; }

        [Display(Name = "صورة الغلاف")]
        public IFormFile CoverImage { get; set; }

        [Display(Name = "ملف PDF")]
        public IFormFile PdfFile { get; set; }

        public IEnumerable<SelectListItem> Subjects { get; set; }
    }
}