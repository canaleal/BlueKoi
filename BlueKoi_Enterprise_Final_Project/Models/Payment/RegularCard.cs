using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Alex
namespace BlueKoi_Enterprise_Final_Project.Models.Payment
{
    /// <summary>
    /// A child class of card that holds information for regular cards
    /// </summary>
    public class RegularCard : Card
    {
        public int CardSection { get; set; } = 0;
        public string CardType { get; } = "This is a regular card.";
    }
}
