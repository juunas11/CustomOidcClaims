using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CustomOidcClaims.Models;
using Microsoft.AspNetCore.Authorization;

namespace CustomOidcClaims.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        [Authorize]
        public IActionResult Claims() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
