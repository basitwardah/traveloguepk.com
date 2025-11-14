using Microsoft.AspNetCore.Mvc;

namespace magazine_app.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

