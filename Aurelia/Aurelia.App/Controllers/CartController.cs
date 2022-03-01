using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace Aurelia.App.Controllers
{
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
            var cart = SessionExtension.Get<List<ShoppingCartItem>>(HttpContext.Session, "cart");
                ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Product.Price * item.quantity);
            return View();
        }

        [Route("Buy/{id}")]
        public IActionResult Buy(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            if (SessionExtension.Get<List<ShoppingCartItem>>(HttpContext.Session, "cart") == null)
            {
                List<ShoppingCartItem> cart = new List<ShoppingCartItem>();
                cart.Add(new ShoppingCartItem { Product = _aureliaDb.Products.Find(id), quantity = 1 });
                SessionExtension.Set(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<ShoppingCartItem> cart = SessionExtension.Get<List<ShoppingCartItem>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].quantity++;
                }
                else
                {
                    cart.Add(new ShoppingCartItem { Product = _aureliaDb.Products.Find(id), quantity = 1 });
                }
                SessionExtension.Set(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        [Route("Remove/{id}")]
        public IActionResult Remove(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            List<ShoppingCartItem> cart = SessionExtension.Get<List<ShoppingCartItem>>(HttpContext.Session, "cart");
            int index = isExist(id);
            cart.RemoveAt(index);
            SessionExtension.Set(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            List<ShoppingCartItem> cart = SessionExtension.Get<List<ShoppingCartItem>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.Id.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

    }
}
