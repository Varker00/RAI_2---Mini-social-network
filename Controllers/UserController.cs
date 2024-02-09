using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RAI_2.Models;
using System.Text;

namespace RAI_2.Controllers
{
    public class UserController : Controller
    {
        private static List<User> users = new List<User>() { new User("admin"), new User("u1"), new User("u2") };
        private bool isAdmin() => HttpContext.Session.Get("username") != null && HttpContext.Session.GetString("username") == "admin";
        private bool isLoggedIn() => HttpContext.Session.Get("username") != null;
        private string? currentUser() => HttpContext.Session.GetString("username");

        private readonly ILogger<HomeController> _logger;
        public UserController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            base.OnActionExecuting(ctx);
            ViewBag.isAdmin = isAdmin();  
        }

        public ActionResult Index()
        {
            if (HttpContext.Session.Get("username") == null) return RedirectToAction("Index", "Home");
            ViewBag.Username=currentUser();
            return View(isAdmin());
        }

        [HttpPost]
        [HttpGet]
        public ActionResult Login(string username)
        {
            if (HttpContext.Session.Get("username") != null) return RedirectToAction("Index");
            if (users.Any(x => x.Username == username))
            {
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("Friends");
            }
            return RedirectToAction("Login", "Home");
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Home");
        }

        //Admin only

        public ActionResult Delete(string username)
        {
            if (!isAdmin()) return RedirectToAction("Index", "Home");
            users.RemoveAll(x => x.Username == username);
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult Add()
        {
            if (HttpContext.Session.Get("username") == null) return RedirectToAction("Login", "Home");
            return View();
        }

        public ActionResult Add(string username)
        {
            if (!isAdmin()) return RedirectToAction("Index", "Home");
            if (!users.Any(x => x.Username == username))
            {
                users.Add(new User(username));
                
            }
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (HttpContext.Session.Get("username") == null) return RedirectToAction("Login", "Home");
            return View(users);
        }


        public ActionResult Init()
        {
            users.RemoveAll(x => x.Username != "admin");
            for(int i= 0; i < 5; i++)
            {
                User u = new User($"user{i}");
                u.AddFriends(users, users.FindAll(x => x.Username != "admin"));
                users.Add(u);
            }
            return RedirectToAction("List");
        }

        //User only
        public ActionResult Friends()
        {
            if (isAdmin()) return RedirectToAction("Index");
            try
            {
                string friends = users.Find(x => x.Username == currentUser()).GetFriendsAsJSON();
                ViewBag.friends = friends;
                //return Json(friends);
                return View();
            }
            catch
            {
                return RedirectToAction("Logout");
            }
        }

        [HttpGet]
        public ActionResult AddFriend(bool? result)
        {
            if (!isLoggedIn()) return RedirectToAction("Index", "Home");
            if(result.HasValue) ViewBag.Result = result.Value;
            return View();
        }

        public ActionResult AddFriendAction(string username)
        {
            if (!isLoggedIn()) return RedirectToAction("Index", "Home");
            ViewBag.Result = true;
            try
            {
                if (username == "admin" || username == HttpContext.Session.GetString("username")) throw new ArgumentException("Cannot add admin or self as friend");
                User currentUser = users.Find(x => x.Username == HttpContext.Session.GetString("username"));
                User friendToAdd = users.Find(x => x.Username == username);
                ViewBag.Result = currentUser.AddFriend(users, friendToAdd);
            }
            catch { ViewBag.Result = false; }


            return RedirectToAction("AddFriend", "User", new { result = ViewBag.Result });
        }

        public ActionResult RemoveFriend(string username)
        {
            if (!isLoggedIn()) return RedirectToAction("Index", "Home");

            try
            {
                User u = users.Find(x => x.Username == currentUser());
                ViewBag.Result = u.RemoveFriend(username);
            }
            catch { ViewBag.Result = false; }

            ViewBag.friends = users.Find(x => x.Username == HttpContext.Session.GetString("username")).GetFriendsAsJSON();

            return View("Friends");
        }

        public IActionResult Export()
        {
            if (!isLoggedIn()) return RedirectToAction("Index", "Home");

            string user = HttpContext.Session.GetString("username");

            string fileContent = users.Find(x=> x.Username == user).GetFriendsAsJSON();

            byte[] fileBytes = Encoding.UTF8.GetBytes(fileContent);
            string fileName = "Friends.txt";

            return File(fileBytes, "text/plain", fileName);
        }

        public IActionResult Import()
        {
            if (!isLoggedIn()) return View();

            return View();
        }

        [HttpPost]
        public IActionResult ImportFriends(IFormFile file)
        {
            if (!isLoggedIn()) return View();
            User u = users.Find(x => x.Username == currentUser());

            if (file == null || Path.GetExtension(file.FileName).ToLower() != ".txt")
            {
                ViewBag.ImportSuccess = false;
                return View("Import");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);
                    stream.Position = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        var jsonString = reader.ReadToEnd();
                        if (!u.ImportFriends(users, jsonString)) throw new Exception();
                    }
                }
            }
            catch
            {
                ViewBag.ImportSuccess = false;
                return View("Import");
            }

            ViewBag.friends = users.Find(x => x.Username == HttpContext.Session.GetString("username")).GetFriendsAsJSON();
            return View("Friends");
        }
    }
}
