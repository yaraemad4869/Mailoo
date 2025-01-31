using Mailo.Data;
using Mailo.Models;
using Mailoo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductVariantController : Controller
    {
        private readonly AppDbContext _context;

        public ProductVariantController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ عرض المتغيرات لكل منتج
        public async Task<IActionResult> Index()
        {
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Include(v => v.Color)
                .Include(v => v.Size)
                .ToListAsync();

            return View(variants);
        }

        // ✅ عرض صفحة إضافة متغير جديد
        public async Task<IActionResult> Create()
        {
            ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "ID", "Name");
            ViewBag.Colors = await _context.Colors.ToListAsync(); // ✅ اجلب قائمة الألوان
            ViewBag.Sizes = await _context.Sizes.ToListAsync();   // ✅ اجلب قائمة الأحجام
            return View();
        }

        // ✅ إضافة المتغير الجديد للمنتج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ProductId, List<int> ColorIds, List<int> SizeIds, List<int> Quantities)
        {
            if (ProductId == 0 || ColorIds == null || SizeIds == null || Quantities == null || !ColorIds.Any() || !SizeIds.Any() || !Quantities.Any())
            {
                TempData["Error"] = "Please select at least one color and one size.";
                return RedirectToAction("Create");
            }

            for (int i = 0; i < ColorIds.Count; i++)
            {
                var variant = new ProductVariant
                {
                    ProductId = ProductId,
                    ColorId = ColorIds[i],
                    SizeId = SizeIds[i],
                    Quantity = Quantities[i]
                };
                _context.ProductVariants.Add(variant);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Variants Added Successfully";
            return RedirectToAction("Index");
        }


        // ✅ حذف متغير معين
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null)
            {
                return NotFound();
            }

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Variant Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
