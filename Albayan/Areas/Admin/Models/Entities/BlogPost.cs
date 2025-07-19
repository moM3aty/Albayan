using System;
using System.ComponentModel.DataAnnotations;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class BlogPost
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PublishDate { get; set; }
        public string Keywords { get; set; } 
    }
}