using Microsoft.AspNetCore.Mvc;

namespace magazine_app.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

