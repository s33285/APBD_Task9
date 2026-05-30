using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User_Panel.Models;

namespace User_Panel.Controllers
{
    [Authorize]  
    public class DashboardController : Controller
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db) => _db = db;

        private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public IActionResult Index()
        {
            int userId = GetUserId();

            var vm = new DashboardViewModel
            {
                UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                Notes = _db.UserNotes
                            .Where(n => n.AppUserId == userId)
                            .OrderByDescending(n => n.CreatedAt)
                            .ToList(),
                NewNote = new AddNoteViewModel()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(string title, string content)
        {
            int userId = GetUserId();

            if (string.IsNullOrWhiteSpace(title))
                ModelState.AddModelError("title", "Title is required.");
            if (string.IsNullOrWhiteSpace(content))
                ModelState.AddModelError("content", "Content is required.");
            if (title?.Length > 200)
                ModelState.AddModelError("title", "Title can be at most 200 characters.");

            if (!ModelState.IsValid)
            {
                var vm = new DashboardViewModel
                {
                    UserEmail = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                    Notes = _db.UserNotes
                                    .Where(n => n.AppUserId == userId)
                                    .OrderByDescending(n => n.CreatedAt)
                                    .ToList(),
                    NewNote = new AddNoteViewModel { Title = title ?? "", Content = content ?? "" }
                };
                return View("Index", vm);
            }

            _db.UserNotes.Add(new UserNote
            {
                AppUserId = userId,
                Title = title!.Trim(),
                Content = content!.Trim(),
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
