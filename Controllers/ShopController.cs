using Microsoft.AspNetCore.Mvc;

namespace EcommerceMVCAdo.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
