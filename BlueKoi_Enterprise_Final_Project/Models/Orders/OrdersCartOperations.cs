using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Alex
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

        public void Delete(int accountId)
        {
            context.OrdersCarts.Remove(context.OrdersCarts.Single(a => a.AccountId == accountId));
            context.SaveChanges();
        }

        public IEnumerable<Order> GetOrders(int orderCartId)
        {
            var data = from st in context.Orders
                   where st.OrderCartId == orderCartId
                   select st;

            return data;

        }

        public void AddOrder(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
        }

        public void DeleteOrders(int orderCartId)
        {
            context.Orders.RemoveRange(context.Orders.Where(x => x.OrderCartId == orderCartId));
            context.SaveChanges();
        }
    }
}
