using Microsoft.AspNetCore.Mvc;
using RAI_2.Models;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace RAI_2.Controllers
{
    public class FileController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private bool IsLoggedIn() => HttpContext.Session.Get("username") != null;
        public FileController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}