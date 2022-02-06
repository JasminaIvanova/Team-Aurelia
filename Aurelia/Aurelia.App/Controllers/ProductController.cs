using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace Aurelia.App.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class ProductController: Controller
    {
        private  ApplicationDbContext _aureliaDb;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _aureliaDb = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult CreateProduct()
        {
           
            ViewData["productCategory"] = new SelectList(_aureliaDb.ProductCategories.ToList(),"Id", "Name");
            return View("CreateProduct");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(ProductViewModel model)
        {
            string uniqueFileName = UploadedFile(model);
            Product product = new Product
            {
                ProductName = model.ProductName,
                Description = model.Description,
                Price = model.Price,
                Quantity = model.Quantity,
                ProductCategory = model.ProductCategory,
                ProductCategoryId = model.ProductCategoryId,
                Brand = model.Brand,
                Image = uniqueFileName

            };
            _aureliaDb.Products.Add(product);
            await _aureliaDb.SaveChangesAsync();
            return View("Index");
           

        }
        private string UploadedFile(ProductViewModel model)
        {
            string uniqueFileName = null;

            if (model.ProductImage != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                uniqueFileName = model.ProductName + "_" + model.ProductImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProductImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

    }
}
