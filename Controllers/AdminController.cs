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
                .ToList();
            return View(users);
        }
    }
}
