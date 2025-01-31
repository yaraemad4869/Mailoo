﻿using Mailo.Data;
using Mailo.Data.Enums;
using Mailo.IRepo;
using Mailo.Models;
using Mailoo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mailo.Repo
{
    public class CartRepo : ICartRepo
    {
        private readonly AppDbContext _db;
        public CartRepo(AppDbContext db)
        {
            _db = db;
        }
        public async Task<Order?> GetOrCreateCart(User user)
        {
            var cart = await _db.Orders
         .Include(o => o.OrderProducts)
             .ThenInclude(op => op.product) 
         .Include(o => o.OrderProducts)
             .ThenInclude(op => op.Variant).ThenInclude(v=>v.Size)
          .Include(o=>o.OrderProducts)
             .ThenInclude(op=>op.Variant).ThenInclude(v=>v.Color)
         .FirstOrDefaultAsync(o => o.UserID == user.ID && o.OrderStatus == OrderStatus.New);

            return cart;
        }

        public async Task<List<Product>> GetProducts(Order order)
        {
            return await _db.OrderProducts.Where(op => op.OrderID == order.ID)
                .Select(op => op.product)
                .ToListAsync();
        }
        public async Task<OrderProduct> IsMatched(Order order, int id, ProductVariant varient)
        {
            return await _db.OrderProducts.FirstOrDefaultAsync(op => op.OrderID == order.ID && op.ProductID == id && op.Variant.Size==varient.Size && op.Variant.Color==varient.Color);
        }

        //public async Task<Order> GetOrder(User user)
        //{
        //    Order? order = await _db.Orders.FirstOrDefaultAsync(o => new { o.UserID == user.ID && o.OrderStatus == OrderStatus.New });
        //        if (order == null)
        //        {
        //            order = new Order { OrderPrice = 0, OrderAddress = user.Address, UserID = user.ID
        //};
        //_db.Orders.Add(order);
        //            _db.SaveChanges();
        //        }

        //    return order;
        //}
        public async Task<Order> GetOrder(User user)
        {
            return await _db.Orders.Include(o=>new{o.Payment, o.employee,o.user } ).FirstOrDefaultAsync(o => o.UserID == user.ID && o.OrderStatus == OrderStatus.New);

        }
        public async Task<List<Order>> GetOrders(User user)
        {

            return _db.Orders.Where(o => o.UserID == user.ID && (o.OrderStatus == OrderStatus.Pending || o.OrderStatus == OrderStatus.Shipped)).ToList();

        }
        public async Task<OrderProduct> ExistingCartItem(int productId, ProductVariant varient, User user)
        {
            return await IsMatched(await GetOrder(user), productId,varient);
        }
        public async void InsertToCart(int OrderId, int ProductId, ProductVariant varient)
        {
            OrderProduct op = new OrderProduct
            {
                OrderID = OrderId,
                ProductID = ProductId,
                VariantID=varient.Id
            };

            await _db.OrderProducts.AddAsync(op);
            _db.SaveChanges();
        }
        public async void DeleteFromCart(int OrderId, int ProductId, ProductVariant varient)
        {
            OrderProduct op = new OrderProduct
            {
                OrderID = OrderId,
                ProductID = ProductId,
                VariantID = varient.Id
            };
            _db.OrderProducts.Remove(op);
            _db.SaveChanges();
        }
    }
}