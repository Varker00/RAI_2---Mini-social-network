using Microsoft.AspNetCore.Mvc;

namespace RAI_2.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }
    }
}
