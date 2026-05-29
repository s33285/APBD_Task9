using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using User_Panel.Models;

namespace User_Panel.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Error() => View(new ErrorViewModel
        {
            RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}
