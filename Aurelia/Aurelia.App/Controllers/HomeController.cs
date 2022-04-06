using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.Extensions.Configuration;


namespace Aurelia.App.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private ApplicationDbContext _aureliaDB;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext aureliaDB, IConfiguration configuration)
        {
            _logger = logger;
            _aureliaDB = aureliaDB;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["productCategory"] = _aureliaDB.ProductCategories.ToList();
            ViewData["products"] = _aureliaDB.Products.ToList();
            string conn = _configuration.GetConnectionString("DefaultConnection");
            string sql = "SELECT ProductId, count(*) as c FROM aurelia.OrderDetails group by ProductId order by c desc limit 6";
            MySqlCommand cmd = new MySqlCommand(sql);
            cmd.Connection = new MySqlConnection(conn);
            cmd.Connection.Open();
            cmd.CommandType = System.Data.CommandType.Text;
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<Product> products = new List<Product>();
            List<string> productsid = new List<string>();
            while (rdr.Read()) 
            {
                productsid.Add((string)rdr["ProductId"]);
            }
            foreach (var item in productsid)
            {
                Product prod = _aureliaDB.Products.Where(x => x.Id == item).FirstOrDefault();
                products.Add(prod);
            }
            return View(products);
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