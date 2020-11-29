using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Orders
{
    [Table("Table_Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Column("AccountId")]
        public int AccountId { get; set; }

        [Column("URL")]
        public string ItemURL { get; set; }

        [Column("OrderStatus")]
        public OrderStatusEnum OrderState { get; set; }

    }
}
