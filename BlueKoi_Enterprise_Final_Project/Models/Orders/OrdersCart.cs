using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Orders
{
    [Table("Table_OrdersCart")]
    public class OrdersCart
    {

        [Key]
        public int Id { get; set; }

        [Column("AccountID")]
        public int AccountId { get; set; }

       public OrdersCart(int accountId)
        {
            AccountId = accountId;
        }
    }
}
