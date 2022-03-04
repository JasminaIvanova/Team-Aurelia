using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Aurelia.App.Controllers
{
    
    [Authorize]
    [Route("Cart")]
    public class CartController : Controller
    {
        private ApplicationDbContext _aureliaDb;
        public CartController(ApplicationDbContext aureliaDb)
        {
            _aureliaDb = aureliaDb;
        }
        [Route("Index")]
        public IActionResult Index()
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
                return View("/Views/Home/Index.cshtml");
            }
            return View();
        }

        [Route("Add/{id}")]
        public IActionResult Add(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");

            var cart = HttpContext.Session.GetString("cart");
            if (cart == null)
            {
                var product = _aureliaDb.Products.Find(id);
                
                List<ShoppingCartItem> listCart = new List<ShoppingCartItem>()
               {
                   new ShoppingCartItem
                   {
                       Product = product,
                       quantity = 1
                   }
               };
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(listCart));

            }
            else
            {
                List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
                bool check = true;
                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product.Id == id)
                    {
                        dataCart[i].quantity++;
                        check = false;
                    }
                }
                if (check)
                {
                    dataCart.Add(new ShoppingCartItem
                    {
                        Product = _aureliaDb.Products.Find(id),
                        quantity = 1
                        
                    });
                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
            }

            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult Update(string id, int quantity)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);
                if (quantity > 0)
                {
                    for (int i = 0; i < dataCart.Count; i++)
                    {
                        if (dataCart[i].Product.Id == id)
                        {
                            dataCart[i].quantity = quantity;
                        }
                    }


                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                }
                var cart2 = HttpContext.Session.GetString("cart");
                return Ok(quantity);
            }
            return BadRequest();

        }

        [Route("Delete/{id}")]
        public IActionResult Delete(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var cart = HttpContext.Session.GetString("cart");
            if (cart != null)
            {
                List<ShoppingCartItem> dataCart = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cart);

                for (int i = 0; i < dataCart.Count; i++)
                {
                    if (dataCart[i].Product.Id == id)
                    {
                        dataCart.Remove(dataCart[i]);
                        if (dataCart.Count == 0)
                        {
                            HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                            cart = null;
                            ViewBag.carts = new List<ShoppingCartItem>();
                            ViewBag.total = 0;
                            return View("/Views/Home/Index.cshtml");
                        }
                    }

                }
                HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(dataCart));
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
