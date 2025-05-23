﻿
using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Aurelia.App.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ProductCategoryController : Controller
    {
        private ApplicationDbContext _aureliaDb;

        public ProductCategoryController(ApplicationDbContext db)
        {
            _aureliaDb = db;
        }
        
        public IActionResult Index()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["products"] = _aureliaDb.Products.ToList();
            return View(_aureliaDb.ProductCategories.ToList());
        }
        public ActionResult CreateCategory()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["products"] = _aureliaDb.Products.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateCategory(ProductCategory productCategory)
        {
            if (ModelState.IsValid)
            {
                _aureliaDb.ProductCategories.Add(productCategory);
                await _aureliaDb.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productCategory);
        }

        public async Task<ActionResult> DeleteCategory(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["products"] = _aureliaDb.Products.ToList();
            var category = await _aureliaDb.ProductCategories.FindAsync(id);

            if (category == null)
            {
                ViewBag.ErrorMessage = $"Category with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                _aureliaDb.Remove(category);
                await _aureliaDb.SaveChangesAsync();
                TempData["delete"] = "Product type has been deleted";
                return RedirectToAction("Index");

            }
        }
       

    }
}
