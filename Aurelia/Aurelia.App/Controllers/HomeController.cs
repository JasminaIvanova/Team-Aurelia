using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Dynamic;

namespace Aurelia.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _aureliaDB;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext aureliaDB)
        {
            _logger = logger;
            _aureliaDB = aureliaDB;
        }

        public IActionResult Index()
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["products"] = _aureliaDB.Products.ToList();
            return View(_aureliaDB.Products.ToList());
        }

        public IActionResult Privacy()
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["products"] = _aureliaDB.Products.ToList();
            return View();
        }

        public ActionResult Detail (string? name)
        {
            if (name == null)
            {
                return NotFound();
            }
            Product product = _aureliaDB.Products.Include(c => c.ProductCategory).FirstOrDefault(c => c.ProductName == name);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}