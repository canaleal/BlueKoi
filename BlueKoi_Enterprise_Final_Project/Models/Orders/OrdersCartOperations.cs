using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Orders
{
    public class OrdersCartOperations : IOrdersCartRepository
    {
        private readonly VirtualStoreDBContext context;
        public OrdersCartOperations(VirtualStoreDBContext context)
        {
            this.context = context;
        }

        public OrdersCart GetAnOrdersCart(int accountId)
        {
            OrdersCart ordersCart = context.OrdersCarts.Where(a => a.AccountId == accountId).FirstOrDefault();
            return ordersCart;
        }

        public void Add(OrdersCart ordersCart)
        {
            context.OrdersCarts.Add(ordersCart);
            context.SaveChanges();
        }

        public IEnumerable<Order> GetOrders(int orderCartId)
        {
            var orders = context.Orders.Where(a => a.OrderCartId == orderCartId);
            return orders;
        }

        public void AddOrder(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
        }  
    }
}
