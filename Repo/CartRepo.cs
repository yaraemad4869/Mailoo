using Mailo.Data;
using Mailo.Data.Enums;
using Mailo.IRepo;
using Mailo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Repo
{
    public class CartRepo : ICartRepo
    {
        private readonly AppDbContext _db;
        public CartRepo(AppDbContext db,IBasicRepo<Order> repo)
        {
            _db = db;
        }
        public async Task<List<Product>> GetProducts(Order order)
        {
            return await _db.OrderProducts.Where(op => op.OrderID == order.ID)
                .Select(op => op.product)
                .ToListAsync();
        }
        public async Task<OrderProduct> IsMatched(Order order,int id)
        {
            return await _db.OrderProducts.FirstOrDefaultAsync(op => op.OrderID == order.ID && op.ProductID == id);
        }
        public async Task<Order> GetOrder(int id)
        {
            return await _db.Orders.FirstOrDefaultAsync(o => o.UserID == id && o.OrderStatus == OrderStatus.New);
        }
        public async Task<OrderProduct> ExistingCartItem(int productId, int userId)
        {
            return await IsMatched(await GetOrder(userId),productId);
        }
        public async void InsertToCart(int OrderId , int ProductId)
        {
            OrderProduct op = new OrderProduct
            {
                OrderID= OrderId,
                ProductID= ProductId
            };
            
            await _db.OrderProducts.AddAsync(op);
            _db.SaveChanges();
        }
		public async void DeleteFromCart(int OrderId, int ProductId)
		{
			OrderProduct op = new OrderProduct
			{
				OrderID = OrderId,
				ProductID = ProductId
			};
             _db.OrderProducts.Remove(op);
			_db.SaveChanges();
		}
	}
}

