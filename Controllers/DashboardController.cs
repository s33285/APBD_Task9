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
            var vm = new DashboardViewModel
            {

                Notes = _db.UserNotes
                    .Where(n => n.AppUserId == GetUserId())
                    .OrderByDescending(n => n.CreatedAt)
                    .ToList(),
                NewNote = new AddNoteViewModel()
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(AddNoteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var vm = new DashboardViewModel
                {
                    Notes = _db.UserNotes
                        .Where(n => n.AppUserId == GetUserId())
                        .OrderByDescending(n => n.CreatedAt)
                        .ToList(),
                    NewNote = model
                };
                return View("Index", vm);
            }

            _db.UserNotes.Add(new UserNote
            {
                AppUserId = GetUserId(),
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
