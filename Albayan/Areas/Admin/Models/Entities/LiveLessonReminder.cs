using Albayan.Areas.Admin.Models.Entities;
using Albayan.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public class LiveLessonReminder
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }
        public int LiveLessonId { get; set; }
        [ForeignKey("LiveLessonId")]
        public virtual LiveLesson LiveLesson { get; set; }
    }
}
