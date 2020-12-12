using BlueKoi_Enterprise_Final_Project.Models.Orders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

//Angela
namespace BlueKoi_Enterprise_Final_Project.Models
{
    /// <summary>
    /// An Account class that will hold all user data including username and password
    /// </summary>
    [Table("Table_Account")]
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Column("Name")]
        public string UserName { get; set; }

        [Column("Email")]
        [Required(ErrorMessage = "Account email is required.")]
        public string UserEmail { get; set; }

        [Column("Password")]
        [Required(ErrorMessage = "Account password is required.")]
        public string UserPassword { get; set; }

        [Column("IsClosed")]
        public bool IsClosed { get; set; }

        [Column("UserState")]
        public UserStateEnum UserState { get; set; }

        [Column("RecentSearch")]
        public string RecentSearch { get; set; }


    }
}
