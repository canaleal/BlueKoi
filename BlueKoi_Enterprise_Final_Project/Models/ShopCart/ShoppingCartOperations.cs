using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.ShopCart
{
    public class ShoppingCartOperations : IShoppingCartRepository
    {
        private readonly VirtualStoreDBContext context;
        public ShoppingCartOperations(VirtualStoreDBContext context)
        {
            this.context = context;
        }


        public void Add(ShoppingCart shoppingCart)
        {
            context.ShoppingCarts.Add(shoppingCart);
            context.SaveChanges();
        }

        public void Delete(int accountId)
        {
            context.ShoppingCarts.Remove(context.ShoppingCarts.Single(a => a.AccountId == accountId));
            context.SaveChanges();
        }

        public ShoppingCart GetAShoppingCart(int accountId)
        {
            ShoppingCart shoppingCart = context.ShoppingCarts.Where(a => a.AccountId == accountId).FirstOrDefault();
            return shoppingCart;
        }

        public IEnumerable<ShoppingCartItem> GetSavedItems(int shoppingCartId)
        {
            return context.ShoppingCartItems.Where(x => x.ShoppingCartId == shoppingCartId).ToList(); ;
        }

        public void AddShoppingCartItem(ShoppingCartItem item)
        {
            context.ShoppingCartItems.Add(item);
            context.SaveChanges();
        }

        public void DeleteShoppingCartItems(int id)
        {
   
            try
            {
                context.ShoppingCartItems.Remove(context.ShoppingCartItems.FirstOrDefault(a => a.Id == id));
                context.SaveChanges();
            }
            catch
            {

            }

        }

        public void DeleteItem(int id)
        {

            try
            {
                context.ShoppingCartItems.Remove(context.ShoppingCartItems.FirstOrDefault(a => a.Id == id));
                context.SaveChanges();
            }
            catch
            {

            }
           

        }
    }
}
