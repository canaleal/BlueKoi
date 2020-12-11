using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Jasmine
namespace BlueKoi_Enterprise_Final_Project.Models.Items
{
    public interface IItemRepository
    {
        Item GetAnItem(int id);

        IEnumerable<Item> GetItems();

        void Add(Item item);

        void Update(Item itemChange);

        void Delete(Item item);

        IEnumerable<Item> GetSpecialItems();

    }
}
