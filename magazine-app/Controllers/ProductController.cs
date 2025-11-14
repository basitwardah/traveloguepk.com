using Microsoft.AspNetCore.Mvc;

namespace magazine_app.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Detail(int? id)
        {
            return View();
        }
    }
}

