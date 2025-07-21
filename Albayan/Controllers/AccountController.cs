using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Models;
using Albayan.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Albayan.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PlatformDbContext _context;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, PlatformDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
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
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    EmailConfirmed = true 
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Student"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Student"));
                    }
                    await _userManager.AddToRoleAsync(user, "Student");

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
                        IsActive = true 
                    };
                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
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
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.ApplicationUserId == user.Id);
                if (student != null && !student.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "تم تعطيل هذا الحساب من قبل الإدارة.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    user.LastLoginDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    return LocalRedirect(returnUrl);
                }
            }

            ModelState.AddModelError(string.Empty, "بيانات الدخول غير صحيحة.");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
