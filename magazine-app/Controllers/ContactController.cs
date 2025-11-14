using Microsoft.AspNetCore.Mvc;

namespace magazine_app.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

