﻿using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        public async Task<IActionResult> Index(string id, string searchString)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            var products = from p in _aureliaDb.Products select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.ProductName!.Contains(searchString));
            }

            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> BuyProduct(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _aureliaDb.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);

        }

    }
}
