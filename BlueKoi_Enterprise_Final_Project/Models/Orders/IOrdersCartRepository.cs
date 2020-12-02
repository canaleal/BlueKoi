using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Orders
{
    public interface IOrdersCartRepository
    {

        OrdersCart GetAnOrdersCart(int accountId);

        void Add(OrdersCart ordersCart);

        void Delete(int accountId);
        
        IEnumerable<Order> GetOrders(int orderCartId);

        void AddOrder(Order order);

    }
}
