using Microsoft.AspNetCore.Mvc;

namespace magazine_app.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

