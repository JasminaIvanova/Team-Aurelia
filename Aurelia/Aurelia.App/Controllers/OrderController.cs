using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Aurelia.App.Controllers
{
    [Route("Order")]
    public class OrderController:Controller
    {
        private ApplicationDbContext _aureliaDb;
        private UserManager<AureliaUser> _userManager;
        public OrderController(ApplicationDbContext aureliaDb, UserManager<AureliaUser> userManager)
        {
            _aureliaDb = aureliaDb;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            return View();
        }

        [Route("Checkout")]
        public IActionResult Checkout()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;
                    ViewBag.total = dataCart.Sum(item => item.Product.Price * item.quantity);
                    return View();
                }
            }
            if (cart == null)
            {
                ViewBag.carts = new List<ShoppingCartItem>();
                ViewBag.total = 0;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(Order order)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var cart = HttpContext.Session.GetString("cart");
            List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (cart != null)
            {
                foreach (var product in dataCart)
                {
                    OrderDetails orderDetails = new OrderDetails();
                    orderDetails.ProductId = product.ProductId;
                    order.date_placed = DateTime.Now;
                    order.Status = "Accepted";
                    order.AureliaUser = await _userManager.GetUserAsync(HttpContext.User);
                    order.UserId = user.Id;
                    orderDetails.ProductId = product.Product.Id;
                    order.OrderDetails.Add(orderDetails);
                }

            }
            order.Id = GetOrderNumber();
            _aureliaDb.Orders.Add(order);
            await _aureliaDb.SaveChangesAsync();
            List<ShoppingCartItem> cart2 = new List<ShoppingCartItem>();
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart2));
            return View();
        }

        public string GetOrderNumber()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            int rowCount = _aureliaDb.Orders.ToList().Count() + 1;
            return rowCount.ToString("000");
        }
    }
}
