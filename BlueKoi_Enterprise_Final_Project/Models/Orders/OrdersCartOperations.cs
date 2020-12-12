using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Orders
{
    /// <summary>
    /// Orders Cart operations class to get, update, add, and delete an order cart and orders from the database
    /// </summary>
    public class OrdersCartOperations : IOrdersCartRepository
    {
        private readonly VirtualStoreDBContext context;
        public OrdersCartOperations(VirtualStoreDBContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Get an accounts order cart from the database
        /// </summary>
        /// <param name="accountId">The account id used to compare to the account id foreign key in the ordercart table</param>
        /// <returns>The order cart of the particular user</returns>
        public OrdersCart GetAnOrdersCart(int accountId)
        {
            OrdersCart ordersCart = context.OrdersCarts.Where(a => a.AccountId == accountId).FirstOrDefault();
            return ordersCart;
        }

        /// <summary>
        /// Add an orders cart to the database
        /// </summary>
        /// <param name="ordersCart">The orders cart that will be saved in the database</param>
        public void Add(OrdersCart ordersCart)
        {
            context.OrdersCarts.Add(ordersCart);
            context.SaveChanges();
        }

        /// <summary>
        /// Delete an orders cart from the database using the account id
        /// </summary>
        /// <param name="accountId">Account id used to compare to the foreign key in orderscart table</param>
        public void Delete(int accountId)
        {
            context.OrdersCarts.Remove(context.OrdersCarts.Single(a => a.AccountId == accountId));
            context.SaveChanges();
        }

        /// <summary>
        /// Get all orders using the orders cart id to compare
        /// </summary>
        /// <param name="orderCartId">The orders cart id used to compare to the foreign key in orders</param>
        /// <returns></returns>
        public IEnumerable<Order> GetOrders(int orderCartId)
        {
            var data = from st in context.Orders
                   where st.OrderCartId == orderCartId
                   select st;

            return data;

        }

        /// <summary>
        /// Add a new order to the database
        /// </summary>
        /// <param name="order">The order that will be saved into the database</param>
        public void AddOrder(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
        }

        /// <summary>
        /// Delete an order from the database using the item id
        /// </summary>
        /// <param name="id">The id used to find the order in the database</param>
        public void DeleteOrder(int id)
        {
            context.Orders.Remove(context.Orders.FirstOrDefault(a => a.Id == id));
            context.SaveChanges();
        }

        /// <summary>
        /// Delete all orders in the database that match the order cart id
        /// </summary>
        /// <param name="orderCartId">The order cart id used to compare to the orders foreign key</param>
        public void DeleteOrders(int orderCartId)
        {
            context.Orders.RemoveRange(context.Orders.Where(x => x.OrderCartId == orderCartId));
            context.SaveChanges();
        }
    }
}
