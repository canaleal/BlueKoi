using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Orders
{
    /// <summary>
    /// An orders class to hold all information for the order including price and item url
    /// </summary>
    [Table("Table_Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Column("OrderCartId")]
        public int OrderCartId { get; set; }

        [Column("URL")]
        public string ItemURL { get; set; }

        [Column("ItemId")]
        public int ItemId { get; set; }

        [Column("Price")]
        public double Price { get; set; }

        [Column("OrderStatus")]
        public OrderStatusEnum OrderState { get; set; }


    }
}
