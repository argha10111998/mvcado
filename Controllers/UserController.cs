using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVCAdo.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View("~/Views/User/RegisterForm.cshtml");
        }
        public IActionResult Login()
        {
            return View("~/Views/User/LoginForm.cshtml");
        }
    }
}
