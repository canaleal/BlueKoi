using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Payment
{
    /// <summary>
    /// The parent class that contains all the infromation about the card used
    /// </summary>
    public class Card
    {
        [Required(ErrorMessage = "Card holder is required.")]
        public string CardHolder { get; set; }
        
        [Required(ErrorMessage = "Card Number is required.")]
        public int CardNumber { get; set; }

        [Required(ErrorMessage = "Expire data is required.")]
        [RegularExpression(@"^((0[1-9])|(1[0-2]))\/((2020)|(20[2-3][0-9]))$",
         ErrorMessage = "Must Match Format (MM/YYYY)")]
        public string ExpireDate { get; set; }

        [Required(ErrorMessage = "CV is required.")]
        [Range(99, 1000, ErrorMessage = "CV must have 3 characters.")]
        public int CVNumber { get; set; }

        [Required(ErrorMessage = "No user is logged in.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "No image is selected.")]
        public string ItemURL { get; set; }

        public string MerchantName { get; set; } = "Blue Koi";
    }
}
