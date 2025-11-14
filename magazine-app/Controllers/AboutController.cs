using Microsoft.AspNetCore.Mvc;

namespace magazine_app.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

