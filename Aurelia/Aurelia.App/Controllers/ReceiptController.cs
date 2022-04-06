using Aurelia.App.Data;
using Aurelia.App.Models;
using Aurelia.App.Reports;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace Aurelia.App.Controllers
{
    public class ReceiptController : Controller
    {
        private ApplicationDbContext _aureliaDb;

        public ReceiptController(ApplicationDbContext aureliaDb)
        {
            _aureliaDb = aureliaDb;
        }
        public ActionResult PrintOrder(int param)
        {
            Order detailsForLastOrder = _aureliaDb.Orders.OrderByDescending(x => x.Id).First();
            List <OrderDetails> orderdet = _aureliaDb.OrderDetails.Where(x => x.OrderId == detailsForLastOrder.Id).ToList();
            List <Product> products = new List<Product>();
            foreach (var item in orderdet) 
            {
                Product prod = _aureliaDb.Products.Where(x => x.Id == item.ProductId).FirstOrDefault();
                prod.Quantity = item.Quantity;
                products.Add(prod);
            }

            OrderReport rpt = new OrderReport();
            return File(rpt.Report(detailsForLastOrder, products), "application/pdf");
        }
    }
}
