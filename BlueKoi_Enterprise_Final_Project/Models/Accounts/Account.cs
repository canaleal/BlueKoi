using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models
{
    [Table("Table_Account")]
    public class Account
    {
        [Key]
        private string Id { get; set; }

        [Column("UserName")]
        [Required(ErrorMessage = "Account username is required.")]
        private string UserName { get; set; }

        [Column("UserName")]
        [Required(ErrorMessage = "Account username is required.")]
        private string UserEmail { get; set; }
        private string UserPassword { get; set; }
        public bool IsClosed { get; set; }
        public UserStateEnum UserState { get; set; }
    }
}
