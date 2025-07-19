using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.ViewModels;

namespace Albayan.Controllers
{
    public class TeacherProfileController : Controller
    {
        private readonly PlatformDbContext _context;

        public TeacherProfileController(PlatformDbContext context)
        {
            _context = context;
        }

        // GET: /TeacherProfile/Index/5
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .Include(t => t.Courses).ThenInclude(c => c.Subject)
                .Include(t => t.Courses).ThenInclude(c => c.Lessons)
                .Include(t => t.Courses).ThenInclude(c => c.StudentCourses)
                .Include(t => t.Ratings).ThenInclude(r => r.Student) 
                .FirstOrDefaultAsync(t => t.Id == id);

            if (teacher == null) return NotFound();

            var viewModel = new TeacherProfileViewModel
            {
                Teacher = teacher,
                Courses = teacher.Courses.Select(c => new CourseInfoViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    CoverImageUrl = c.CoverImageUrl,
                    SubjectName = c.Subject.Name,
                    LessonsCount = c.Lessons.Count,
                    TotalHours = c.TotalHours
                }).ToList(),
                TotalCourses = teacher.Courses.Count,
                TotalStudents = teacher.Courses.SelectMany(c => c.StudentCourses).Select(sc => sc.StudentId).Distinct().Count(),
                AverageRating = teacher.Ratings.Any() ? teacher.Ratings.Average(r => r.Rating) : 0,
                RatingsCount = teacher.Ratings.Count,
                RecentRatings = teacher.Ratings.OrderByDescending(r => r.RatingDate).Take(5).ToList(),
                NewRating = new SubmitRatingViewModel { TeacherId = teacher.Id }
            };

            return View(viewModel);
        }

        // POST: /TeacherProfile/SubmitRating
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRating(SubmitRatingViewModel newRating)
        {
            int currentStudentId = 1; 

            if (ModelState.IsValid)
            {
                var existingRating = await _context.TeacherRatings
                    .FirstOrDefaultAsync(r => r.TeacherId == newRating.TeacherId && r.StudentId == currentStudentId);

                if (existingRating != null)
                {
                    existingRating.Rating = newRating.Rating;
                    existingRating.Comment = newRating.Comment;
                    existingRating.RatingDate = DateTime.UtcNow;
                }
                else
                {
                    var rating = new TeacherRating
                    {
                        TeacherId = newRating.TeacherId,
                        StudentId = currentStudentId,
                        Rating = newRating.Rating,
                        Comment = newRating.Comment,
                        RatingDate = DateTime.UtcNow
                    };
                    _context.TeacherRatings.Add(rating);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { id = newRating.TeacherId });
        }
    }
}
