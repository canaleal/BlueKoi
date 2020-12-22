using BlueKoi_Enterprise_Final_Project.Models.Items;
using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlueKoi_Enterprise_Final_Project.Models.Orders;
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Data
{
    /// <summary>
    /// The db context of the virtual store database
    /// </summary>
    public class VirtualStoreDBContext : DbContext
    {
        public VirtualStoreDBContext(DbContextOptions<VirtualStoreDBContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Item> Items { get; set; }

        public DbSet<OrdersCart> OrdersCarts { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    }
}
