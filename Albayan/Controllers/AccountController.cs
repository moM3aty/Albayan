using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Albayan.Models;
using Albayan.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentProfileViewModel = Albayan.ViewModels.StudentProfileViewModel;

namespace Albayan.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly PlatformDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, PlatformDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var viewModel = new RegisterViewModel
            {
                Grades = new SelectList(await _context.Grades.ToListAsync(), "Id", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ModelState.Remove("Grades");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email, FullName = model.FullName };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    try
                    {
                        var student = new Student
                        {
                            FullName = user.FullName,
                            Email = user.Email,
                            PhoneNumber = model.PhoneNumber,
                            RegistrationDate = DateTime.UtcNow,
                            GradeId = model.GradeId,
                            ApplicationUserId = user.Id,
                            City = model.City,             
                            Country = model.Country,
                        };

                        _context.Students.Add(student);
                        await _context.SaveChangesAsync();

                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception)
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError("", "حدث خطأ أثناء حفظ البيانات. يرجى المحاولة مرة أخرى.");
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            model.Grades = new SelectList(await _context.Grades.ToListAsync(), "Id", "Name", model.GradeId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = Url.IsLocalUrl(returnUrl) ? returnUrl : null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة");
                ViewData["ReturnUrl"] = returnUrl;
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

                if (student != null)
                {
                    student.LastAccessDate = DateTime.Now;
                    student.IsActive = true;
                    await _context.SaveChangesAsync();
                }

                if (Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }

            ModelState.AddModelError(string.Empty, "فشل في تسجيل الدخول. تأكد من صحة البيانات.");
            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student != null)
            {
                student.IsActive = false;
                await _context.SaveChangesAsync();

                Console.WriteLine($"Logout - Updated IsActive: {student.IsActive}");
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var student = await _context.Students
                .Include(s => s.Grade)
                .FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);

            if (student == null)
            {
                return NotFound("لم يتم العثور على ملف الطالب المرتبط بهذا الحساب.");
            }

            var studentCourses = await _context.StudentCourses.Include(sc => sc.Course).Where(sc => sc.StudentId == student.Id).ToListAsync();
            var certificates = await _context.Certificates.Include(c => c.Course).Where(c => c.StudentId == student.Id).ToListAsync();
            var totalHours = studentCourses.Sum(sc => (sc.ProgressPercentage / 100.0) * sc.Course.TotalHours);
            double averageGrade = certificates.Any() ? certificates.Average(c => c.GradePercentage) : 0;

            var viewModel = new StudentProfileViewModel
            {
                Student = student,
                CoursesProgress = studentCourses.Select(sc => new StudentCourseProgressViewModel { CourseTitle = sc.Course.Title, ProgressPercentage = sc.ProgressPercentage, Status = sc.ProgressPercentage >= 100 ? "مكتمل" : (sc.ProgressPercentage > 0 ? "قيد التقدم" : "لم يبدأ") }).ToList(),
                Certificates = certificates.Select(c => new CertificateInfoViewModel { CertificateId = c.Id, CourseTitle = c.Course.Title, IssueDate = c.IssueDate, CertificateGuid = c.CertificateGuid }).ToList(),
                CompletedCourses = studentCourses.Count(sc => sc.ProgressPercentage >= 100),
                CertificatesCount = certificates.Count,
                TotalLearningHours = Math.Round(totalHours, 1),
                AverageGrade = Math.Round(averageGrade, 2)
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(StudentProfileViewModel model)
        {
            var student = await _context.Students.FindAsync(model.Student.Id);
            if (student == null) return NotFound();

            student.FullName = model.Student.FullName;
            student.PhoneNumber = model.Student.PhoneNumber;
            student.Email = model.Student.Email;
            student.City = model.Student.City;
            student.Country = model.Student.Country;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var user = await _userManager.FindByEmailAsync(student.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    await _userManager.ResetPasswordAsync(user, token, model.Password);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Profile), new { id = student.Id });
        }
    }
}
