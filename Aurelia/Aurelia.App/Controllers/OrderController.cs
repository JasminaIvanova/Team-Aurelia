using Microsoft.AspNetCore.Mvc;

namespace Aurelia.App.Controllers
{
    public class OrderController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
