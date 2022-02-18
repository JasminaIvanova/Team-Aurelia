using Aurelia.App.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Aurelia.App.Controllers
{
    public class ClientController : Controller
    {
        private ApplicationDbContext _aureliaDb;
        public ClientController(ApplicationDbContext db)
        {
            _aureliaDb = db;
        }
        public IActionResult Index(string id)
        {
            
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }
            return View(_aureliaDb.Products.Where(x => x.ProductCategoryId == id).ToList());
            
        }
    }
}
