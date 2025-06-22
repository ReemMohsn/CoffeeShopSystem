using happinesCafe.DATA;
using happinesCafe.Models;
using happinesCafe.Models.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace happinesCafe.Controllers
{
    public class ProductMangementController : Controller
    {
        private readonly CaffeeSystemContext _context;
        private readonly IHostingEnvironment _host;
        public ProductMangementController(CaffeeSystemContext context, IHostingEnvironment host)
        {

            _context = context;
            _host = host;
        }
        public IActionResult DisplayProduct()
        {
            IEnumerable<Product> productsList = _context.Products.Include(c => c.IdCategoryNavigation).ToList();
            return View(productsList);
        }
        public async Task<IActionResult> New()
        {
            var categories = await _context.Categories.ToListAsync();
            var sizes = await _context.Sizes.ToListAsync();

            var viewModel = new CreateProductViewModel
            {
                Categories = categories.Select(c => new SelectListItem { Value = c.IdCategory.ToString(), Text = c.NameCategory }).ToList(),
                AvailableSizes = sizes.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList()
            };

            // Initialize the SizesAndPrices dictionary with available sizes
            foreach (var size in sizes)
            {
                viewModel.SizesAndPrices.Add(size.Id, 0); // Initialize price to 0 or null
            }

            return View(viewModel);
        }


        // POST: /Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(CreateProductViewModel viewModel)
        {
            var _uniqueFileName = "t";
            string uniqueFileName = String.Empty;
            if (viewModel.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_host.WebRootPath, "imges"); // Ensure this folder exists
                uniqueFileName = viewModel.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                await viewModel.ImageFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                _uniqueFileName = uniqueFileName;


            }

            var newProduct = new Product
            {
                NameProduct = viewModel.Name,
                IdCategory = viewModel.CategoryId,
                Picture = _uniqueFileName,
                About = viewModel.Description,
                CreatedDate = DateTime.Now,
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            // Save product sizes and prices
            foreach (var sizeId in viewModel.SizesAndPrices.Keys)
            {
                if (viewModel.SizesAndPrices[sizeId] > 0) // Only save if a price is provided
                {
                    var productSize = new ProductsSize
                    {
                        IdProduct = newProduct.IdProduct,
                        IdSize = sizeId,
                        Price = viewModel.SizesAndPrices[sizeId]
                    };
                    _context.ProductsSizes.Add(productSize);
                    await _context.SaveChangesAsync();
                }
            }

            TempData["successData"] = "product has been added successfully";

            return RedirectToAction("DisplayProduct");
            //if (ModelState.IsValid)
            //{
            //    
            //     // Redirect to a success page
            //}

            //// If ModelState is not valid, repopulate the categories and sizes
            //var categories = await _context.Categories.ToListAsync();
            //var sizes = await _context.Sizes.ToListAsync();
            //viewModel.Categories = categories.Select(c => new SelectListItem { Value = c.IdCategory.ToString(), Text = c.NameCategory }).ToList();
            //viewModel.AvailableSizes = sizes.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList();

            //return View(viewModel);
        }
        // GET: /Product/Show/5
        public async Task<IActionResult> Show(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.IdCategoryNavigation) //  بتضمين الفئة
                .Include(p => p.ProductsSizes) //  بتضمين الأحجام والأسعار
                .ThenInclude(ps => ps.IdSizeNavigation)    //  بتضمين معلومات الحجم
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // GET: /Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(p => p.IdCategoryNavigation).Include(p => p.ProductsSizes)
                  .ThenInclude(ps => ps.IdSizeNavigation).FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _context.Categories.ToListAsync();
            var sizes = await _context.Sizes.ToListAsync();
            var viewModel = new EditProductViewModel
            {
                Id = product.IdProduct,
                Name = product.NameProduct,
                CategoryId = product.IdCategory,
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.IdCategory.ToString(),
                    Text = c.NameCategory
                }).ToList(),
                Description = product.About,
                ExistingPicture = product.Picture,
                AvailableSizes = sizes.Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name }).ToList(),
                SizesAndPrices = product.ProductsSizes.ToDictionary(ps => ps.IdSize, ps => ps.Price)
            };
            foreach (var size in sizes)
            {
                if (!viewModel.SizesAndPrices.ContainsKey(size.Id))
                {
                    viewModel.SizesAndPrices.Add(size.Id, 0);
                }
            }
            return View(viewModel);
        }

        // POST: /Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }
            var productToUpdate = await _context.Products.FindAsync(id);
            if (productToUpdate == null)
            {
                return NotFound();
            }
            productToUpdate.NameProduct = viewModel.Name;
            productToUpdate.IdCategory = viewModel.CategoryId;
            productToUpdate.About = viewModel.Description;
            if (viewModel.ImageFile != null)
            {

                if (!string.IsNullOrEmpty(productToUpdate.Picture))
                {
                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imges", productToUpdate.Picture);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                var _uniqueFileName = "t";
                string uniqueFileName = String.Empty;
                if (viewModel.ImageFile != null)
                {
                    string uploadsFolder = Path.Combine(_host.WebRootPath, "imges"); // Ensure this folder exists
                    uniqueFileName = viewModel.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    await viewModel.ImageFile.CopyToAsync(new FileStream(filePath, FileMode.Create));
                    _uniqueFileName = uniqueFileName;


                }
                productToUpdate.Picture = _uniqueFileName;
            }
            _context.Products.Update(productToUpdate);
            var existingProductSizes = await _context.ProductsSizes.Where(ps => ps.IdProduct == id).ToListAsync();
            _context.ProductsSizes.RemoveRange(existingProductSizes);
            foreach (var sizeId in viewModel.SizesAndPrices.Keys)
            {
                if (viewModel.SizesAndPrices[sizeId] > 0) // Only save if price is greater than 0
                {
                    var productSize = new ProductsSize
                    {
                        IdProduct = id,
                        IdSize = sizeId,
                        Price = viewModel.SizesAndPrices[sizeId]
                    };
                    _context.ProductsSizes.Add(productSize);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("DisplayProduct", "ProductMangement");
            //if (ModelState.IsValid)
            //{
            //    try
            //    {

            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!ProductExists(viewModel.Id))
            //        {
            //            return NotFound();
            //        }
            //        else
            //        {
            //            throw;
            //        }
            //    }
            //    return RedirectToAction("DisplayProduct", "ProductMangement");
            //}
            //return View();
        }

        //private bool ProductExists(int id)
        //{
        //    throw new NotImplementedException();
        //}

        // GET: /Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Products.Include(p => p.IdCategoryNavigation)
                .FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.Include(p => p.ProductsSizes).FirstOrDefaultAsync(m => m.IdProduct == id);
            if (product == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(product.Picture))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "imges", product.Picture);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                //product.Picture = null;



            }
            if (product.ProductsSizes != null)
            {
                _context.ProductsSizes.RemoveRange(product.ProductsSizes);
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("DisplayProduct", "ProductMangement");
        }

    }
}

