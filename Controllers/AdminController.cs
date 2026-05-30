using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using User_Panel.Models;

namespace User_Panel.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class AdminController : Controller
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db) => _db = db;

        public IActionResult Index()
        {
            var users = _db.AppUsers
                .OrderBy(u => u.CreatedAt)
                .Select(u => new AdminUserRow
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt,
                    NoteCount = u.Notes.Count
                })
                .ToList();

            return View(users);
        }
    }

    public class AdminUserRow
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int NoteCount { get; set; }
    }
}

