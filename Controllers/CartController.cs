using Mailo.Data;
using Mailo.Data.Enums;
using Mailo.IRepo;
using Mailo.Models;
using Mailo.Repo;
using Mailoo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepo _order;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _db;

        public CartController(ICartRepo order, IUnitOfWork unitOfWork, AppDbContext db)
        {
            _order = order;
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity?.Name);
            if (user == null) return RedirectToAction("Login", "Account");

            Order? cart = await _order.GetOrCreateCart(user);
            if (cart == null || cart.OrderProducts == null) return View();
            if (cart.OrderProducts == null)
            {
                cart.OrderProducts = new List<OrderProduct>();
            }
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity.Name);
            var cart = await _order.GetOrCreateCart(user);
            if (cart != null)
            {
                cart.OrderProducts.Clear();
                _unitOfWork.orders.Delete(cart);
            }
            else
            {
                return View("Index");

            }

            return RedirectToAction("Index");
        }
        public IActionResult GetColorsForSize(int productId, int sizeId)
        {
            var colors = _db.ProductVariants
                .Where(v => v.ProductId == productId && v.SizeId == sizeId && v.Quantity > 0)
                .Select(v => new { colorId = v.ColorId, colorName = v.Color.ColorName })
                .Distinct()
                .ToList();

            return Json(colors);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(int productId, string color, string size, int quantity = 1)
        {
            Console.WriteLine(color, size);
            // Log the incoming parameters for debugging
            Console.WriteLine($"AddProduct called with ProductId: {productId}, Color: {color}, Size: {size}, Quantity: {quantity}");

            // Get the current user
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity?.Name);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in.";
                return RedirectToAction("Login", "Account");
            }
            Product? product = await _unitOfWork.products.GetByIDWithIncludes(productId,p=>p.Variants);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                Console.WriteLine("Product not found.");
                return BadRequest(TempData["ErrorMessage"]);
            }

            // Normalize the color and size values
            color = color.Trim().ToLower();
            size = size.Trim().ToLower();

            // Fetch the variant based on color and size
            var variant = await _db.ProductVariants
                .Include(v => v.Color)
                .Include(v => v.Size)
                .FirstOrDefaultAsync(v => v.ProductId == productId &&
                                          v.ColorId.ToString() == color &&
                                          v.SizeId.ToString() == size);

            if (variant == null)
            {
                TempData["ErrorMessage"] = "Variant not found.";
                Console.WriteLine($"Variant not found for ProductId: {product.ID}, Color: {color}, Size: {size}"); // Debugging
                return BadRequest(TempData["ErrorMessage"]);
            }

            // Get or create the user's cart
            var cart = await _order.GetOrCreateCart(user);
            if (cart == null)
            {
                cart = new Order
                {
                    UserID = user.ID,
                    OrderPrice = product.TotalPrice * quantity,
                    FinalPrice= product.TotalPrice * quantity,
                    OrderAddress = user.Address,
                    OrderProducts = new List<OrderProduct>()
                };
                _unitOfWork.orders.Insert(cart);
                await _unitOfWork.CommitChangesAsync();
            }
            else
            {
                cart.OrderPrice += product.TotalPrice * quantity;
                cart.FinalPrice += product.TotalPrice * quantity;
            }
            // Check if the product with the same variant is already in the cart
            if (cart.OrderProducts.Any(op => op.ProductID == productId && op.VariantID == variant.Id))
            {
                TempData["ErrorMessage"] = "Product with this variant is already in the cart.";
                Console.WriteLine($"Product {product.ID} with variant {variant.Id} is already in the cart."); // Debugging
                return BadRequest(TempData["ErrorMessage"]);
            }
            OrderProduct op = new OrderProduct
            {
                ProductID = productId,
                VariantID = variant.Id,
                OrderID = cart.ID,
                Quantity = quantity
            };
            cart.OrderProducts.Add(op);
            _db.SaveChanges();
            _unitOfWork.orders.Update(cart);
            
            TempData["SuccessMessage"] = "Product added to cart successfully.";
            return RedirectToAction("Index_U", "User");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProduct(int productId, int variantId)
        {
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity?.Name);
            var cart = await _order.GetOrCreateCart(user);
            if (cart == null) return View("Index");

            var orderProduct = cart.OrderProducts?.FirstOrDefault(op => op.ProductID == productId && op.VariantID == variantId);
            if (orderProduct != null)
            {
                var product = await _unitOfWork.products.GetByID(productId);
                cart.OrderPrice -= product.TotalPrice * orderProduct.Quantity;
                cart.FinalPrice-= product.TotalPrice * orderProduct.Quantity;
                cart.OrderProducts?.Remove(orderProduct);
                await _unitOfWork.CommitChangesAsync();
            }
            else
            {
                TempData["ErrorMessage"] = "Product not found";
                return BadRequest(TempData["ErrorMessage"]);
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewOrder()
        {
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity?.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var existingOrderItem = await _order.GetOrCreateCart(user);

            if (existingOrderItem == null || (existingOrderItem.OrderStatus != OrderStatus.New))
            {
                TempData["ErrorMessage"] = "Cart is already ordered";
                return BadRequest(TempData["ErrorMessage"]);
            }
            if (existingOrderItem.OrderProducts != null && existingOrderItem.OrderProducts.Any())
            {
                var varients = existingOrderItem.OrderProducts.Where(op => op.OrderID == existingOrderItem.ID)
                    .Select(op => op.Variant)
                    .ToList();
                foreach (var v in varients)
                {
                    v.Quantity -= existingOrderItem.OrderProducts.FirstOrDefault(op => op.VariantID == v.Id).Quantity;
                }

                existingOrderItem.OrderStatus = OrderStatus.Pending;
                _unitOfWork.orders.Update(existingOrderItem);
                Payment payment = new Payment
                {
                    OrderId = existingOrderItem.ID,
                    UserID = user.ID,
                    TotalPrice = existingOrderItem.FinalPrice
                };
                existingOrderItem.Payment = payment;
                _unitOfWork.payments.Insert(payment);
                _unitOfWork.orders.Update(existingOrderItem);

                TempData["Success"] = "Cart Has Been Ordered Successfully";
                return RedirectToAction("GetOrders");
            }
            return View("Index");
        }
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity?.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var orders = await _order.GetOrders(user);

            if (orders != null)
            {
                return View(orders);
            }
            else
            {
                return View("Index");

            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int OrderId)
        {
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity?.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var order = await _unitOfWork.orders.GetByIDWithIncludes(OrderId,
                order => order.employee,
                order => order.Payment,
                order => order.user,
                order => order.OrderProducts
             );
            if (order != null)
            {
                var orderProducts = await _db.OrderProducts.Include(op => op.product).Include(op=>op.Variant).Where(op => op.OrderID == order.ID).ToListAsync();
                if (orderProducts != null && orderProducts.Any())
                {
                    var varients = orderProducts
                     .Select(op => op.Variant)
                     .ToList();
                    foreach (var v in varients)
                    {
                        v.Quantity += orderProducts.FirstOrDefault(op => op.VariantID == v.Id).Quantity;
                    }
                    _db.OrderProducts.RemoveRange(orderProducts);
                    _db.SaveChanges();
                }

                if (order.Payment != null)
                {
                    _unitOfWork.payments.Delete(order.Payment);
                }
                _unitOfWork.orders.Delete(order);
                return RedirectToAction("GetOrders");
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> ApplyPromoCode(string promoCode)
        {
            User? user = await _unitOfWork.userRepo.GetUser(User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _order.GetOrCreateCart(user);
            if (cart == null || !cart.OrderProducts.Any())
            {
                TempData["PromoMessage"] = "Your cart is empty.";
                TempData["PromoStatus"] = "Error";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrEmpty(cart.PromoCodeUsed))
            {
                if (cart.PromoCodeUsed == promoCode)
                {
                    TempData["PromoMessage"] = "This promo code has already been applied to your order.";
                }
                else
                {
                    TempData["PromoMessage"] = "A promo code has already been applied to this order. You cannot apply another one.";
                }
                TempData["PromoStatus"] = "Error";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(promoCode))
            {
                TempData["PromoMessage"] = "Please enter a promo code.";
                TempData["PromoStatus"] = "Error";
                return RedirectToAction("Index");
            }

            var promo = await _db.PromoCodes.FirstOrDefaultAsync(p => p.Code == promoCode && p.ExpiryDate >= DateTime.Now);
            if (promo == null)
            {
                TempData["PromoMessage"] = "Invalid or expired promo code.";
                TempData["PromoStatus"] = "Error";
                return RedirectToAction("Index");
            }

            double discountAmount = cart.OrderPrice * (promo.DiscountPercentage / 100);
            cart.FinalPrice -= discountAmount;
            _unitOfWork.orders.Update(cart);
            cart.PromoCodeUsed = promo.Code;
            _unitOfWork.orders.Update(cart);

            TempData["PromoMessage"] = $"Promo code applied successfully! You saved {discountAmount:C}.";
            TempData["PromoStatus"] = "Success";

            return RedirectToAction("Index");
        }
    }
}