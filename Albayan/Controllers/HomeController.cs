﻿using Albayan.Areas.Admin.Data;
using Albayan.Models;
using Albayan.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Albayan.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlatformDbContext _context;

        public HomeController(PlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch Teachers
            var teachers = await _context.Teachers
                .Include(t => t.Subjects)
                .Include(t => t.Courses).ThenInclude(c => c.StudentCourses)
                .Include(t => t.Ratings)
                .Select(t => new PublicTeacherViewModel
                {
                    Id = t.Id,
                    FullName = t.FullName,
                    ProfileImageUrl = t.ProfileImageUrl,
                    JobTitle = t.JobTitle,
                    ExperienceSummary = t.ExperienceSummary,
                    ExperienceYears = t.ExperienceYears,
                    SubjectsSummary = t.Subjects.Any() ? string.Join("، ", t.Subjects.Select(s => s.Name)) : "مواد متعددة",
                    TotalStudents = t.Courses.SelectMany(c => c.StudentCourses).Select(sc => sc.StudentId).Distinct().Count(),
                    AverageRating = t.Ratings.Any() ? t.Ratings.Average(r => r.Rating) : 0
                })
                .ToListAsync();


            var stages = await _context.Stages
                .Include(s => s.Grades).ThenInclude(g => g.Subjects)
                .Include(s => s.Grades).ThenInclude(g => g.Students)
                .Select(s => new PublicStageViewModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    GradeRange = s.Grades.Any() ? $"الصفوف {s.Grades.Min(g => g.Id)} - {s.Grades.Max(g => g.Id)}" : "لا توجد صفوف",
                    SubjectNames = s.Grades.SelectMany(g => g.Subjects).Select(sub => sub.Name).Distinct().ToList(),
                    StudentCount = s.Grades.SelectMany(g => g.Students).Count()
                })
                .ToListAsync();

            var educationalMaterials = await _context.EducationalMaterials
                .Include(m => m.Grade)
                .Include(m => m.Subject)
                .Select(m => new PublicEducationalMaterialViewModel
                {
                    Id = m.Id,
                    Title = m.Title,
                    Description = m.Description.Length > 100 ? m.Description.Substring(0, 100) + "..." : m.Description,
                    CoverImageUrl = m.CoverImageUrl,
                    PageCount = m.PageCount,
                    GradeName = m.Grade.Name,
                    SubjectName = m.Subject.Name
                }).ToListAsync();
            var viewModel = new HomeViewModel
            {
                Teachers = teachers,
                Stages = stages,
                EducationalMaterials = educationalMaterials
            };

            return View(viewModel);
        }

        public IActionResult blog()
        {
            return View();
        }
        public IActionResult liveCourses()
        {
            return View();
        }
        public IActionResult login()
        {
            return View();
        }
   
        public IActionResult signUp()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
