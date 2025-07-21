using Microsoft.AspNetCore.Http;
using Albayan.Areas.Admin.Models.Entities;
using System.ComponentModel.DataAnnotations;
using System;

namespace Albayan.Areas.Admin.Models.ViewModels
{
    public class BlogPostIndexViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime PublishDate { get; set; }
        public string ImageUrl { get; set; }
        public string Keywords { get; set; }
    }

    public class BlogPostFormViewModel
    {
        public BlogPost BlogPost { get; set; }

        [Display(Name = "صورة المقال")]
        public IFormFile PostImage { get; set; }
    }
}
