using Mailo.Data;
using Mailo.Data.Enums;
using Mailo.IRepo;
using Mailo.Models;
using Mailo.Repo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Controllers
{
    public class CartController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ICartRepo _order;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public CartController(UserManager<User> userManager, ICartRepo order, IUnitOfWork unitOfWork, AppDbContext db)
        {
            _userManager = userManager;
            _order = order;
            _db = db;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found");
            }
            return View(await _order.GetProducts(await _order.GetOrder(user.ID)));
        }
        public async Task<IActionResult> New()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(Product product)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var existingCartItem = await _order.ExistingCartItem(product.ID, user.ID);

            if (existingCartItem != null)
            {
                return BadRequest("Product is already in the cart.");
            }
            Order o = await _order.GetOrder(user.ID);
             _order.InsertToCart(o.ID, existingCartItem.ProductID);
            
            TempData["Success"] = "Product Has Been Added Successfully";
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> NewOrder()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            var existingOrderItem = await _order.GetOrder(user.ID);
            //.FirstOrDefaultAsync(w => w.UserId == user.Id && w.ProductId == productId);

            if (existingOrderItem == null||(existingOrderItem.OrderStatus!=OrderStatus.New))
            {
                // If the product is already in the wishlist, you may want to return a message
                return BadRequest("Cart is already ordered.");
            }
            var products = await _db.OrderProducts.Where(op => op.OrderID == existingOrderItem.ID)
                .Select(op => op.product)
                .ToListAsync();
            foreach (var product in products)
            {
                product.Quantity -= 1;
            }
            existingOrderItem.OrderStatus = OrderStatus.Pending;
            existingOrderItem.DeliveryFee = 100;
            TempData["Success"] = "Cart Has Been Ordered Successfully";
            return RedirectToAction("Index");
        }
		[HttpDelete, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteFromCart(int id = 0)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return Unauthorized();
			}
			var existingCartItem = await _order.ExistingCartItem(id, user.ID);

			if (existingCartItem == null)
			{
				// If the product is already in the wishlist, you may want to return a message
				return BadRequest("Cart is already ordered.");
			}
            _order.DeleteFromCart(existingCartItem.OrderID, existingCartItem.ProductID);

			TempData["Success"] = "Cart Has Been Ordered Successfully";
			return RedirectToAction("Index");

		}
	}

}
