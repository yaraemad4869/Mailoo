using Mailo.Data;
using Mailo.IRepo;
using Mailo.Models;
using Mailo.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Controllers
{
    public class WishlistController : Controller
    {
        private readonly IAddToWishlistRepo _wishlist;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public WishlistController(AppDbContext db,IAddToWishlistRepo wishlist, IUnitOfWork unitOfWork)
        {
            _wishlist = wishlist;
            _unitOfWork = unitOfWork;
            _db = db;
        }
        public async Task<IActionResult> Index()
        {
            User? user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return NotFound("User not found");
            }
            return View(await _wishlist.GetProducts(user.ID));
        }
        public async Task<IActionResult> New()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(Product product)
        {
            User? user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }
            //int userId = User.Identity.GetUserId();
            var existingWishlistItem = await _wishlist.ExistingWishlistItem(product.ID, user);
            //.FirstOrDefaultAsync(w => w.UserId == user.Id && w.ProductId == productId);

            if (existingWishlistItem != null)
            {
                return BadRequest("Product is already in the wishlist.");
            }

            // Add product to the wishlist
            var wishlistItem = new Wishlist
            {
                UserID = user.ID,
                ProductID = product.ID
            };

            _unitOfWork.wishlists.Insert(wishlistItem);
            TempData["Success"] = "Product Has Been Added Successfully";
            return RedirectToAction("Index");
        }
        //public async Task<IActionResult> New(int id)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return Unauthorized();
        //    }
        //    var existingWishlistItem = await _wishlist.ExistingWishlistItem(id, user.ID);
        //    //.FirstOrDefaultAsync(w => w.UserId == user.Id && w.ProductId == productId);

        //        if (existingWishlistItem != null)
        //        {
        //            // If the product is already in the wishlist, you may want to return a message
        //            return BadRequest("Product is already in the wishlist.");
        //        }

        //        // Add product to the wishlist
        //        var wishlistItem = new Wishlist
        //        {
        //            UserID = user.ID,
        //            ProductID = id
        //        };

        //        _unitOfWork.wishlists.Insert(wishlistItem);
        //        TempData["Success"] = "Product Has Been Added Successfully";
        //        return RedirectToAction("Index");
        //    }

        public async Task<IActionResult> Delete(int id)
	{
            User? user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
		{
			return Unauthorized();
		}
		var existingWishlistItem = await _wishlist.ExistingWishlistItem(id, user);
		//.FirstOrDefaultAsync(w => w.UserId == user.Id && w.ProductId == productId);

		if (existingWishlistItem == null)
		{
			// If the product is already in the wishlist, you may want to return a message
			return BadRequest("Product is not in the wishlist.");
		}

		// Add product to the wishlist
		var wishlistItem = new Wishlist
		{
			UserID = user.ID,
			ProductID = id
		};

		_unitOfWork.wishlists.Delete(wishlistItem);
		TempData["Success"] = "Product Has Been Added Successfully";
		return RedirectToAction("Index");
	}
}
        
}
