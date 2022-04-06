using Aurelia.App.Data;
using Aurelia.App.Models;
using Aurelia.App.Reports;
using Aurelia.App.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace Aurelia.App.Controllers
{
    [Authorize]
    [Route("Order")]
    public class OrderController : Controller
    {
        private ApplicationDbContext _aureliaDb;
        private UserManager<AureliaUser> _userManager;
        private readonly IWebHostEnvironment _oHostEnvironment;
        private readonly IEmailService emailService;

        public OrderController(ApplicationDbContext aureliaDb, UserManager<AureliaUser> userManager, IWebHostEnvironment oHostEnvironment, IEmailService emailService)
        {
            _aureliaDb = aureliaDb;
            _userManager = userManager;
            _oHostEnvironment = oHostEnvironment;
            this.emailService = emailService;
        }

        [Route("Index")]
        public IActionResult Index(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            ViewData["orders"] = _aureliaDb.Orders.ToList();
            return View();
        }

        [Route("Checkout")]

        public IActionResult Checkout()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var cart = HttpContext.Session.GetString("cart");
            List<OrderDetails> ordersDetail = _aureliaDb.OrderDetails.ToList();
            if (cart != null)
            {
                List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);

                if (dataCart.Count > 0)
                {
                    ViewBag.carts = dataCart;
                    ViewBag.total = dataCart.Sum(item => item.Product.Price * item.quantity);
                    foreach (var item in dataCart)
                    {
                        Product product = _aureliaDb.Products.FirstOrDefault(x => x.Id == item.Product.Id);
                        product.Quantity -= item.quantity;
                        _aureliaDb.Update(product);
                        _aureliaDb.SaveChanges();
                    }
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
        public async Task<IActionResult> Checkout(Order order, int param)
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
                    orderDetails.Quantity = product.quantity;
                }

            }

            order.Id = GetOrderNumber();
            _aureliaDb.Orders.Add(order);
            await _aureliaDb.SaveChangesAsync();
            this.emailService.SendEmail(new Message((new string[] { order.Email }).ToList(), "Information for order №" + order.Id,
                $"Thank you for your order! \n\n" +
                $"Your order details are displayed below :)\n" +
                $"Date placed: {order.date_placed} \n" +
                $"Status: {order.Status} \n" +
                $"Payment method: {order.PaymentMethod} \n" +
                $"Address:{order.Address} \n" +
                $"Delivery method: {order.DeliveryMethod} \nPhone number:  {order.PhoneNumber}"));
            List<ShoppingCartItem> cart2 = new List<ShoppingCartItem>();
            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(cart2));
            return RedirectToAction(nameof(Index));
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
