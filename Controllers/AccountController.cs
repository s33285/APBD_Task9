using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User_Panel.Models;

namespace User_Panel.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly PasswordService _passwordService;

        public AccountController(AppDbContext db, PasswordService passwordService)
        {
            _db = db;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (_db.AppUsers.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "An account with this email already exists.");
                return View(model);
            }

            var user = new AppUser
            {
                Email = model.Email,
                PasswordHash = _passwordService.HashPassword(model.Password),
                Role = "User",
                CreatedAt = DateTime.UtcNow
            };

            _db.AppUsers.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Account created successfully. You can now log in.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _db.AppUsers.FirstOrDefault(u => u.Email == model.Email);


            if (user == null || !_passwordService.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid login credentials. Please try again.");
                return View(model);
            }

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Email),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role)
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() => View();
    }
}
