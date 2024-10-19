using Mailo.Data.Enums;
using Mailo.Data;
using Mailo.IRepo;
using Mailo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;

namespace Mailo.Controllers
{
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        public UserController(IUnitOfWork unitOfWork, AppDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        #region Admin 
        public async Task<IActionResult> Index_A()
        {
            return View(await _unitOfWork.products.GetAll());
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
                    product.dbImage = stream.ToArray();
                }
                _unitOfWork.products.Insert(product);
                TempData["Success"] = "Product Has Been Added Successfully";
                return RedirectToAction("Index_A");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.products.Update(product);
                TempData["Success"] = "Product Has Been Updated Successfully";
                return RedirectToAction("Index_A");
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

        [HttpPost, ActionName("Delete")]
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
                    return RedirectToAction("Index_A");
                }
            }
            return NotFound();

        }
        #endregion



        #region Client
        public async Task<IActionResult> Index_U()
        {
            return View(await _unitOfWork.products.GetAll());
        }

        #endregion














    }
}
