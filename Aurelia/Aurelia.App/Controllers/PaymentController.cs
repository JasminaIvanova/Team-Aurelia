using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;

namespace Aurelia.App.Controllers
{
    public class PaymentController : Controller
    {
        private ApplicationDbContext _aureliaDb;
        public PaymentController(ApplicationDbContext aureliaDb)
        {
            _aureliaDb = aureliaDb;
        }
        [HttpGet]
        public IActionResult Payment()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var cart = HttpContext.Session.GetString("cart");
            List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
            ViewBag.PaymentAmount = (long?)((dataCart.Sum(item => item.Product.Price * item.quantity)) * 100);
            return View();
        }

        [HttpPost]
        public IActionResult Payment(string stripeToken, string stripeEmail)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var cart = HttpContext.Session.GetString("cart");
            List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);



            var options = new ChargeCreateOptions
            {
                Amount = (long?)((dataCart.Sum(item => item.Product.Price * item.quantity)) * 100),
                Currency = "USD",
                Description = "Aurelia Products",
                Source = stripeToken,
                ReceiptEmail = stripeEmail
            };
            var chargeService = new ChargeService();
            Charge charge = chargeService.Create(options);

            if(charge.Status == "succeeded")
            {
                return RedirectToAction("Checkout", "Order");
            }
            return View();
        }

        
    }
}
