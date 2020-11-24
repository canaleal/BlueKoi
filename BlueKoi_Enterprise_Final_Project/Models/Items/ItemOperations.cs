using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Items
{
    public class ItemOperations : IItemRepository
    {
        private readonly VirtualStoreDBContext context;
        public ItemOperations(VirtualStoreDBContext context)
        {
            this.context = context;
        }

        public void Add(Item item)
        {
            context.Items.Add(item);
            context.SaveChanges();
        }

        public void Delete(Item item)
        {
            context.Items.Remove(item);
            context.SaveChanges();
        }

        public Item GetAnItem(int id)
        {
            return context.Items.Find(id);
        }

        public IEnumerable<Item> GetItems()
        {
            return context.Items.ToList();
        }

        public void Update(Item itemChange)
        {
            var item = context.Items.Attach(itemChange);
            item.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }
    }
}
