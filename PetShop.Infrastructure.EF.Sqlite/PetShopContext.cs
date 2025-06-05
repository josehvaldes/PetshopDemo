using Microsoft.EntityFrameworkCore;
using PetShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetShop.Infrastructure.EF.Sqlite
{
    /// <summary>
    /// This class represents the database context for the Pet Shop application.
    /// </summary>
    public class PetShopContext : DbContext
    {
        /// <summary>
        /// This is the DbSet for the Product entity, which represents the products in the Pet Shop.
        /// </summary>
        public DbSet<Product> Products { get; set; }
        public DbSet<Sale> Sales { get; set; }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            //=> options.UseSqlite("Data Source=blogging.db?cache=shared");
            => options.UseSqlite("DataSource=file::memory:?cache=shared");

        /// <summary>
        /// This method is used to configure the model that was discovered by convention from the entity types
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Product>()
                .HasKey(p => p.guid); // Set the primary key for the Product entity

            modelBuilder.Entity<Sale>()
                .HasKey( s => s.saleid);
        }
    }
}
