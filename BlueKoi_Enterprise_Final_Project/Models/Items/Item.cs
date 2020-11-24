using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Items
{
    [Table("Table_Item")]
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Column("AccountID")]
        [Required(ErrorMessage = "Seller Id is required.")]
        public int AccountId { get; set; }

        [Column("Name")]
        [Required(ErrorMessage = "Item name is required.")]
        [MaxLength(50, ErrorMessage = "Max length is 50 characters.")]
        public string ItemName { get; set; }

        [Column("URL")]
        [Required(ErrorMessage = "Item url is required.")]
        public string ItemURL{ get; set; }

        [Column("Description")]
        [Required(ErrorMessage = "Item description is required.")]
        [MinLength(10, ErrorMessage = "Minimum length is 10 characters.")]
        [MaxLength(100, ErrorMessage = "Max length is 100 characters.")]
        public string ItemDescription{ get; set; }

        [Column("Price")]
        [Range(1, 1000, ErrorMessage = "Price should be between 1 and 1000")]
        public double ItemPrice { get; set; }
    }
}
