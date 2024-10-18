using Mailo.Models;
using Microsoft.AspNetCore.Mvc;
using Mailo.Repo;
using Mailo.IRepo;
using Mailo.Data;
using Mailo.Data.Enums;


namespace Mailo.Controllers
{
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        public ProductController(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.products.GetAll());
        }

        public async Task<IActionResult> Getpants()
        {
            var products = _context.Products.Where(p => p.Category == Product_Categories.Pants);
            return View(products);
        }
        public async Task<IActionResult> Gethoddies()
        {
            var products = _context.Products.Where(p => p.Category == Product_Categories.hoodi);
            return View(products);
        }
        public async Task<IActionResult> New()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.clientFile != null)
                { 
                    MemoryStream stream = new MemoryStream();
                    product.clientFile.CopyTo(stream);
                    product.dbImage=stream.ToArray();
                }
                _unitOfWork.products.Insert(product);
                TempData["Success"] = "Product Has Been Added Successfully";
                return RedirectToAction("Index");
            }
            else
                return View(product);
        }


        public async Task<IActionResult> Edit(int id = 0)
        {
            if (id != 0)
            {
                return View(await _unitOfWork.products.GetByID(id));
            }
            return NotFound();
        }
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.products.Update(product);
                TempData["Success"] = "Product Has Been Updated Successfully";
                return RedirectToAction("Index");
            }
            return View(product);
        }


        public async Task<IActionResult> Delete(int id = 0)
        {
            if (id != 0)
            {
                return View(await _unitOfWork.products.GetByID(id));
            }
            return NotFound();
        }

        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id = 0)
        {
            if (id != 0)
            {
                var product = await _unitOfWork.products.GetByID(id);
                if (product != null)
                {
                    _unitOfWork.products.Delete(product);
                    TempData["Success"] = "product Has Been Deleted Successfully";
                    return RedirectToAction("Index");
                }
            }
            return NotFound();

        }
    }
}












