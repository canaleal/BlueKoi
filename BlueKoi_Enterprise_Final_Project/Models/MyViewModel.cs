using BlueKoi_Enterprise_Final_Project.Models.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models
{
    public class MyViewModel
    {
        public Account account { get; set; }
        public IEnumerable<Item> items { get; set; }


    }
}
