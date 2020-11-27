using BlueKoi_Enterprise_Final_Project.Models.Items;
using BlueKoi_Enterprise_Final_Project.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueKoi_Enterprise_Final_Project.Models.Data
{
    public class VirtualStoreDBContext : DbContext
    {
        public VirtualStoreDBContext(DbContextOptions<VirtualStoreDBContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Item> Items { get; set; }
        
    }
}
