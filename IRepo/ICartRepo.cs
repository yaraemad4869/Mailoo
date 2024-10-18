using Mailo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mailo.IRepo
{
    public interface ICartRepo
    {

        Task<List<Product>> GetProducts(Order order);
        Task<OrderProduct> IsMatched(Order order, int id);
        Task<Order> GetOrder(int id);
        Task<OrderProduct> ExistingCartItem(int productId, int userId);
		void InsertToCart(int iD, int productID);
        void DeleteFromCart(int OrderId, int ProductId);

	}
}
