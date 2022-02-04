using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Aurelia.App.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ProductController: Controller
    {
        private  ApplicationDbContext _aureliaDb;

        public ProductController(ApplicationDbContext db)
        {
            _aureliaDb = db;
        }
        public ActionResult CreateProduct()
        {
           
            ViewData["productCategory"] = new SelectList(_aureliaDb.ProductCategories.ToList(),"Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(Product product, IFormFile file)
        {
            /*var product = _aureliaDb.Product.FirstOrDefault(c => c.Name == product.Name);
            if (searchProduct != null)
            {
                ViewBag.message = "This product is already exist";
                ViewData["productTypeId"] = new SelectList(_db.ProductTypes.ToList(), "Id", "ProductType");
                ViewData["TagId"] = new SelectList(_db.SpecialTags.ToList(), "Id", "Name");
                return View(product);
            }*/

            string fileName = $"{Directory.GetCurrentDirectory()}{@"\Files"}" + "\\" + file.FileName;
           

            using (FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }
            if (ModelState.IsValid)
            {
                product.Image = fileName;
                _aureliaDb.Products.Add(product);
                await _aureliaDb.SaveChangesAsync();
                return RedirectToAction("CreateProduct");
            }
            return View("CreateProduct");
        }
        //public IActionResult Index()
        //{
        //    return View(_aureliaDb.Products.ToList());
        //}

    }
}
