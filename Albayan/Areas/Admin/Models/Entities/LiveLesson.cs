using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Albayan.Areas.Admin.Models.Entities
{
    public enum LiveLessonStatus { Upcoming, Live, Recorded }
    public enum LivePlatform { Zoom, GoogleMeet, MicrosoftTeams, Other }

    public class LiveLesson
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }

        [Display(Name = "موعد البدء")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        public DateTime StartTime { get; set; }

        [Display(Name = "المدة (بالدقائق)")]
        public int DurationMinutes { get; set; }
        public int MaxStudents { get; set; }
        public LivePlatform Platform { get; set; }
        public string MeetingUrl { get; set; }
        public LiveLessonStatus Status { get; set; }
        public int RemindersCount { get; set; }

        // Foreign Keys
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        [Required(ErrorMessage = "يجب اختيار الصف الدراسي")]
        [Display(Name = "الصف الدراسي")]
        public int GradeId { get; set; }

        // Navigation Properties
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
        [ForeignKey("GradeId")]
        public virtual Grade Grade { get; set; }

        public virtual ICollection<LiveLessonReminder> LiveLessonReminders { get; set; }
    }
}
