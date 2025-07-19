using Albayan.Areas.Admin.Data;
using Albayan.Areas.Admin.Models.Entities;
using Albayan.Areas.Admin.Models.ViewModels;
using Albayan.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Albayan.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LiveLessonsController : Controller
    {
        private readonly PlatformDbContext _context;
        private readonly IFileService _fileService;
        private readonly TimeZoneInfo _localZone;

        public LiveLessonsController(PlatformDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
            _localZone = TimeZoneInfo.Local;

        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.UtcNow;
            var allLessons = await _context.LiveLessons
                                    .Include(l => l.Teacher)
                                    .Include(l => l.Subject)
                                    .OrderByDescending(l => l.StartTime)
                                    .ToListAsync();

            var viewModel = new LiveLessonIndexViewModel
            {
                UpcomingLessons = allLessons.Where(l => l.StartTime > now).Select(MapToListItemViewModel),
                LiveNowLessons = allLessons.Where(l => l.StartTime <= now && l.StartTime.AddMinutes(l.DurationMinutes) > now).Select(MapToListItemViewModel),
                RecordedLessons = allLessons.Where(l => l.StartTime.AddMinutes(l.DurationMinutes) <= now).Select(MapToListItemViewModel)
            };

            return View(viewModel);
        }

        private LiveLessonListItemViewModel MapToListItemViewModel(LiveLesson lesson)
        {
            return new LiveLessonListItemViewModel
            {
                Id = lesson.Id,
                Title = lesson.Title,
                TeacherName = lesson.Teacher.FullName,
                SubjectName = lesson.Subject.Name,
                StartTime = lesson.StartTime,
                DurationMinutes = lesson.DurationMinutes, 
                CoverImageUrl = lesson.CoverImageUrl,
                MeetingUrl = lesson.MeetingUrl
            };
        }

        #region Other Actions
        public IActionResult Create()
        {
            var viewModel = new LiveLessonFormViewModel
            {
                LiveLesson = new LiveLesson { StartTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddDays(1), _localZone) },
                Teachers = new SelectList(_context.Teachers, "Id", "FullName"),
                Subjects = new SelectList(_context.Subjects, "Id", "Name")
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LiveLessonFormViewModel viewModel)
        {
            ModelState.Remove("Subjects");
            ModelState.Remove("Teachers");
            ModelState.Remove("LiveLesson.Subject");
            ModelState.Remove("LiveLesson.Teacher");
            ModelState.Remove("LiveLesson.CoverImageUrl");
            if (ModelState.IsValid)
            {
                var liveLesson = viewModel.LiveLesson;
                if (viewModel.CoverImage != null)
                {
                    liveLesson.CoverImageUrl = await _fileService.SaveFileAsync(viewModel.CoverImage, "livelessons");
                }
                liveLesson.StartTime = TimeZoneInfo.ConvertTimeToUtc(liveLesson.StartTime, _localZone);

                _context.Add(liveLesson);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            viewModel.Teachers = new SelectList(_context.Teachers, "Id", "FullName", viewModel.LiveLesson.TeacherId);
            viewModel.Subjects = new SelectList(_context.Subjects, "Id", "Name", viewModel.LiveLesson.SubjectId);
            return View(viewModel);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var liveLesson = await _context.LiveLessons.FindAsync(id);
            if (liveLesson == null) return NotFound();
            liveLesson.StartTime = TimeZoneInfo.ConvertTimeFromUtc(liveLesson.StartTime, _localZone);

            var viewModel = new LiveLessonFormViewModel
            {
                LiveLesson = liveLesson,
                Teachers = new SelectList(_context.Teachers, "Id", "FullName", liveLesson.TeacherId),
                Subjects = new SelectList(_context.Subjects, "Id", "Name", liveLesson.SubjectId)
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LiveLessonFormViewModel viewModel)
        {
            if (id != viewModel.LiveLesson.Id) return NotFound();

            ModelState.Remove("CoverImage");
            ModelState.Remove("Subjects");
            ModelState.Remove("Teachers");
            ModelState.Remove("LiveLesson.Subject");
            ModelState.Remove("LiveLesson.Teacher");
            ModelState.Remove("LiveLesson.CoverImageUrl");
            if (ModelState.IsValid)
            {
                var lessonToUpdate = await _context.LiveLessons.FindAsync(id);
                if (lessonToUpdate == null) return NotFound();

                if (viewModel.CoverImage != null)
                {
                    _fileService.DeleteFile(lessonToUpdate.CoverImageUrl);
                    lessonToUpdate.CoverImageUrl = await _fileService.SaveFileAsync(viewModel.CoverImage, "livelessons");
                }


                lessonToUpdate.Title = viewModel.LiveLesson.Title;
                lessonToUpdate.Description = viewModel.LiveLesson.Description;
                lessonToUpdate.StartTime = viewModel.LiveLesson.StartTime;
                lessonToUpdate.DurationMinutes = viewModel.LiveLesson.DurationMinutes;
                lessonToUpdate.MeetingUrl = viewModel.LiveLesson.MeetingUrl;
                lessonToUpdate.TeacherId = viewModel.LiveLesson.TeacherId;
                lessonToUpdate.SubjectId = viewModel.LiveLesson.SubjectId;
                lessonToUpdate.StartTime = TimeZoneInfo.ConvertTimeToUtc(viewModel.LiveLesson.StartTime, _localZone);

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.LiveLessons.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            viewModel.Teachers = new SelectList(_context.Teachers, "Id", "FullName", viewModel.LiveLesson.TeacherId);
            viewModel.Subjects = new SelectList(_context.Subjects, "Id", "Name", viewModel.LiveLesson.SubjectId);
            return View(viewModel);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var liveLesson = await _context.LiveLessons.FindAsync(id);
            if (liveLesson != null)
            {
                _fileService.DeleteFile(liveLesson.CoverImageUrl);
                _context.LiveLessons.Remove(liveLesson);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
