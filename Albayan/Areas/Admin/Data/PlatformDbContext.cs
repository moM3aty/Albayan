using Albayan.Areas.Admin.Models.Entities;
using Albayan.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Albayan.Areas.Admin.Data
{
    public class PlatformDbContext : IdentityDbContext<ApplicationUser>
    {
        public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options) { }

        public DbSet<Stage> Stages { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<LiveLesson> LiveLessons { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<EducationalMaterial> EducationalMaterials { get; set; }
        public DbSet<SalesOutlet> SalesOutlets { get; set; }
        public DbSet<TeacherRating> TeacherRatings { get; set; }
        public DbSet<LessonMaterial> LessonMaterials { get; set; }
        public DbSet<LessonAttachment> LessonAttachments { get; set; }
        public DbSet<HomeworkSubmission> HomeworkSubmissions { get; set; }
        public DbSet<LiveLessonReminder> LiveLessonReminders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Subjects)
                .WithMany(s => s.Teachers)
                .UsingEntity(j => j.ToTable("TeacherSubjects"));

            modelBuilder.Entity<StudentCourse>()
                .HasKey(sc => new { sc.StudentId, sc.CourseId });

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Exam)
                .WithOne(e => e.Course)
                .HasForeignKey<Exam>(e => e.CourseId);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Course)
                .WithMany(co => co.Certificates)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.Student)
                .WithMany(s => s.Certificates)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Course)
                .WithMany(c => c.StudentCourses)
                .HasForeignKey(sc => sc.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentCourse>()
                .HasOne(sc => sc.Student)
                .WithMany(s => s.StudentCourses)
                .HasForeignKey(sc => sc.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherRating>()
                .HasOne(r => r.Teacher)
                .WithMany(t => t.Ratings)
                .HasForeignKey(r => r.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeacherRating>()
                .HasOne(r => r.Student)
                .WithMany(s => s.GivenRatings)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                 .HasOne(s => s.ApplicationUser)
                 .WithOne()
                 .HasForeignKey<Student>(s => s.ApplicationUserId);

            modelBuilder.Entity<LessonMaterial>()
                .HasOne(lm => lm.Subject)
                .WithMany(s => s.LessonMaterials)
                .HasForeignKey(lm => lm.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Grades)
                .WithMany(g => g.Subjects)
                .UsingEntity(j => j.ToTable("SubjectGrades"));

            modelBuilder.Entity<EducationalMaterial>()
                .HasOne(em => em.Grade)
                .WithMany(g => g.EducationalMaterials)
                .HasForeignKey(em => em.GradeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EducationalMaterial>()
                .HasOne(em => em.Subject)
                .WithMany(s => s.EducationalMaterials)
                .HasForeignKey(em => em.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EducationalMaterial>()
                .HasMany(em => em.SalesOutlets)
                .WithMany(so => so.AvailableMaterials)
                .UsingEntity(j => j.ToTable("MaterialSalesOutlets"));

            modelBuilder.Entity<HomeworkSubmission>()
                .HasOne(s => s.Student)
                .WithMany(st => st.HomeworkSubmissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HomeworkSubmission>()
                .HasOne(s => s.Lesson)
                .WithMany(l => l.HomeworkSubmissions)
                .HasForeignKey(s => s.LessonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LiveLessonReminder>()
                .HasOne(r => r.Student)
                .WithMany(s => s.LiveLessonReminders) 
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<LiveLessonReminder>()
                .HasOne(r => r.LiveLesson)
                .WithMany(l => l.LiveLessonReminders) 
                .HasForeignKey(r => r.LiveLessonId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
