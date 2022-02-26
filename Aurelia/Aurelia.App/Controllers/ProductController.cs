using Aurelia.App.Data;
using Aurelia.App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Index ()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            return View(_aureliaDb.Products.Include(x => x.ProductCategory).ToList());
        }
        public IActionResult CreateProduct()
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(),"Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
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
                return RedirectToAction(nameof(Index));
            }
            return View("Index");

        }

        private string getImageName(string imagePath)
        {
            var imagePathArr = imagePath.Split("_");
            return imagePathArr[imagePathArr.Length - 1];
        }
        public async Task<IActionResult> EditProduct(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            Product product = await _aureliaDb.Products.FindAsync(id);
            ProductViewModel productViewModel = new ProductViewModel()
            {
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                Brand = product.Brand,
                ProductCategory = product.ProductCategory,
                ProductCategoryId = product.ProductCategoryId
            };

            if (product == null || productViewModel == null)
            {
                return NotFound();
            }
            ViewBag.Id = id;
            ViewBag.Image = product.Image;
            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct(string id, ProductViewModel productViewModel)
        {
           if(ModelState.IsValid)
            {
                try
                {
                    Product product = _aureliaDb.Products.Find(id);
                    product.ProductName = productViewModel.ProductName;
                    product.Description = productViewModel.Description;
                    product.Price = productViewModel.Price;
                    product.Quantity = productViewModel.Quantity;
                    product.Brand = productViewModel.Brand;
                    product.ProductCategory = productViewModel.ProductCategory;
                    product.ProductCategoryId = productViewModel.ProductCategoryId;
                    product.Image = productViewModel.ImagePath == null ? UploadedFile(productViewModel) : 
                        (productViewModel.ProductImage == null ? productViewModel.ImagePath : (getImageName(productViewModel.ImagePath)
                        .ToLower()
                        .Equals(productViewModel.ProductImage.FileName.ToLower()) ? productViewModel.ImagePath : 
                        UploadedFile(productViewModel)));
                    _aureliaDb.Update(product);
                    await _aureliaDb.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(id))
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        public async Task<IActionResult> Details(string id)
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
        public async Task<IActionResult> Delete(string id)
        {
            ViewData["productCategory"] = _aureliaDb.ProductCategories.ToList();
            ViewData["productCategorySelectable"] = new SelectList(_aureliaDb.ProductCategories.ToList(), "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }

            Product product = await _aureliaDb.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Product product = _aureliaDb.Products.FirstOrDefault(x => x.Id == id);
            _aureliaDb.Products.Remove(product);
            await _aureliaDb.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(string id)
        {
            return _aureliaDb.Products.Any(e => e.Id == id);
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
