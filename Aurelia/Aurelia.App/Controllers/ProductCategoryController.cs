
using Microsoft.AspNetCore.Mvc;

namespace Aurelia.App.Controllers
{
    [Area("Admin")]
    public class ProductCategoryController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

    }
}
