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
        public IActionResult CreateProduct()
        {
           
            ViewData["productCategory"] = new SelectList(_aureliaDb.ProductCategories.ToList(),"Id", "Name");
            return View("CreateProduct");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(Product product, IFormFile file)
        {
            
                //validaciq
                string fileName = $"{Directory.GetCurrentDirectory()}{@"\Images"}" + "\\" + file.FileName;
                using (FileStream fileStream = System.IO.File.Create(fileName))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                }
                product.Image = fileName;
                

                if (ModelState.IsValid) 
                {
                    _aureliaDb.Products.Add(product);
                    await _aureliaDb.SaveChangesAsync();
                    return View("CreateProduct");
                }
            return View("Index");

           

        }
       

    }
}
