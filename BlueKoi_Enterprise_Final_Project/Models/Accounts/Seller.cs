using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Accounts
{
    public class Seller : Account
    {

        private string SellerDescription { get; set; }
        private bool CanSell { get; set; }
        public List<int> ItemsSoldId { get; set; }
    }
}
