using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.ShopCart
{
    public interface IShoppingCartRepository
    {
        ShoppingCart GetAShoppingCart(int accountId);

        void Add(ShoppingCart shoppingCart);

        void Delete(int accountId);

        IEnumerable<ShoppingCartItem> GetSavedItems(int shoppingCartId);

        void AddShoppingCartItem(ShoppingCartItem item);
    }
}
