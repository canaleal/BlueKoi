using BlueKoi_Enterprise_Final_Project.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//Jasmine
namespace BlueKoi_Enterprise_Final_Project.Models.Items
{
    /// <summary>
    /// Item operations class to get, update, add, and delete an item (Image) from the database
    /// </summary>
    public class ItemOperations : IItemRepository
    {
        private readonly VirtualStoreDBContext context;
        public ItemOperations(VirtualStoreDBContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Add an item (image) to the database
        /// </summary>
        /// <param name="item">The item that will be added to the database</param>
        public void Add(Item item)
        {
            context.Items.Add(item);
            context.SaveChanges();
        }

        /// <summary>
        /// Delete an item from the database given the item instance
        /// </summary>
        /// <param name="item">The item that will be deleted from the database</param>
        public void Delete(Item item)
        {
            context.Items.Remove(item);
            context.SaveChanges();
        }

        /// <summary>
        /// Get an item (Image) from the database given the ID of the item
        /// </summary>
        /// <param name="id">The id of the item we will get from the databse</param>
        /// <returns>The item from the database</returns>
        public Item GetAnItem(int id)
        {
            return context.Items.Find(id);
        }

        /// <summary>
        /// Get all items (Images) from the database that has the 'regular' type
        /// </summary>
        /// <returns>A list of all the items that have the type regular from the database</returns>
        public IEnumerable<Item> GetItems()
        {
            return context.Items.Where(x=>x.Type == "Regular").ToList();
        }

        /// <summary>
        /// Update an item in the databse with an updated instance of the item
        /// </summary>
        /// <param name="itemChange">The item with the new updated information</param>
        public void Update(Item itemChange)
        {
            var item = context.Items.Attach(itemChange);
            item.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
        }

        /// <summary>
        /// Get all items (Images) with the special type from the database
        /// </summary>
        /// <returns>A list of all the items from the database with the special tag</returns>
        public IEnumerable<Item> GetSpecialItems()
        {
            List<Item> list = context.Items.Where(x => x.Type == "Special").ToList();
            return list;
        }

        public void AddBulkItems(IEnumerable<Item> items)
        {
            context.Items.AddRange(items);
            context.SaveChanges();
        }
    }
}
