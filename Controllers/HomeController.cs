using Microsoft.AspNetCore.Mvc;
using RAI_2.Models;
using System.Diagnostics;

namespace RAI_2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            if (HttpContext.Session.Get("username") == null) return View();
            return RedirectToAction("Index", "User");
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.Get("username") == null) return View();
            return RedirectToAction("Index", "User");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}