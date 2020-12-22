using BlueKoi_Enterprise_Final_Project.Models.Items;
using BlueKoi_Enterprise_Final_Project.Models.Orders;
using BlueKoi_Enterprise_Final_Project.Models.Payment;
using BlueKoi_Enterprise_Final_Project.Models.ShopCart;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.ViewModels
{
    public class ViewModelData
    {

        public Account Account { get; set; }

        public int AccountId { get; set; }

        public int OrderCartId { get; set; }

        public IEnumerable<Item> Items { get; set; }

        public JToken ItemsSimple1 { get; set; }
        public JToken ItemsSimple2 { get; set; }

        public Item Item { get; set; }

        public int ItemId { get; set; }

        public string ItemSimple { get; set; }

        public Card Card { get; set; }

        public int CardType { get; set; }

        public Order Order { get; set; }

        public IEnumerable<Order> Orders { get; set; }

        public IEnumerable<ShoppingCartItem> ShoppingCartItems { get; set; }

    }
}
