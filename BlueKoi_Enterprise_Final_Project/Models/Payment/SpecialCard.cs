using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Payment
{
    /// <summary>
    /// A child class of card that holds information for special cards
    /// </summary>
    public class SpecialCard : Card
    {
        public int CardSection { get; set; } = 1;
        public string CardType { get; set; } = "This is a specialized card.";

    }
}
