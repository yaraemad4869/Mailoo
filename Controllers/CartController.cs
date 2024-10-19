using Mailo.Data;
using Mailo.Data.Enums;
using Mailo.IRepo;
using Mailo.Models;
using Mailo.Repo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Controllers
{
    //[Authorize(Roles ="Client")]
    public class CartController : Controller
    {
        
        private readonly ICartRepo _order;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;
        public CartController(ICartRepo order, IUnitOfWork unitOfWork, AppDbContext db)
        {
            //_userManager = userManager;
            _order = order;
            _db = db;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            User? user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found");
            }
            return View(await _order.GetProducts(await _order.GetOrder(user)));
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(Product product)
        {
            var user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }

            if (user.orders != null)
            {
                var existingCartItem = await _order.ExistingCartItem(product.ID, user);

                if (existingCartItem != null)
                {
                    return BadRequest("Product is already in the cart.");
                }

                Order o = await _order.GetOrder(user);

                _order.InsertToCart(o.ID, existingCartItem.ProductID);
            }
            else
            {
                Order order = new Order
                {
                    OrderPrice = product.TotalPrice,
                    OrderAddress = user.Address,
                    UserID = user.ID
                };
                _db.Orders.Add(order);
                _db.SaveChanges();
                OrderProduct op = new OrderProduct
                {
                    OrderID=order.ID,
                    ProductID=product.ID
                };
                _order.InsertToCart(order.ID, op.ProductID);
            }
            TempData["Success"] = "Product Has Been Added Successfully";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewOrder()
        {
            var user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }
            var existingOrderItem = await _order.GetOrder(user);
            //.FirstOrDefaultAsync(w => w.UserId == user.Id && w.ProductId == productId);

            if (existingOrderItem == null || (existingOrderItem.OrderStatus != OrderStatus.New))
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
            TempData["Success"] = "Cart Has Been Ordered Successfully";
            return RedirectToAction("Index");
        }
        [HttpDelete, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFromCart(int id = 0)
        {
            var user = _db.Users.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }
            var existingCartItem = await _order.ExistingCartItem(id, user);

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