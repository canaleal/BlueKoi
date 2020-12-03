using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Payment
{
    public class Card
    {
        [Required(ErrorMessage = "Card holder is required.")]
        public string CardHolder { get; set; }

        [Required(ErrorMessage = "Card Number is required.")]
        public int CardNumber { get; set; }

        [Required(ErrorMessage = "Expire is required.")]
        [RegularExpression(@"^(?:0[1-9]|1[0-2])(\d{2})$",
         ErrorMessage = "Characters are not allowed.")]
        public int ExpireDate { get; set; }

        [Required(ErrorMessage = "CV is required.")]
        [Range(99, 1000, ErrorMessage = "CV must have 3 characters.")]
        public int CVNumber { get; set; }

        [Required(ErrorMessage = "No user is logged in.")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "No image is selected.")]
        public string ItemURL { get; set; }

        public bool IsSaleValid { get; set; } = false;

        public void ConfirmSale()
        {
            //Use Stripe online processing tool. I dont have money for it right now
            IsSaleValid = true;
        }

    }
}
