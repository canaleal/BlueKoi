using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.ShopCart
{
    [Table("Table_ShoppingCartItem")]
    public class ShoppingCartItem
    {
        [Key]
        public int Id { get; set; }

        [Column("ShoppingCartId")]
        public int ShoppingCartId { get; set; }

        [Column("URL")]
        public string ItemURL { get; set; }

        [Column("Price")]
        public double Price { get; set; }

       
    }
}
