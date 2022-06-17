using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ShoppingCart.Models 
{ 
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options)
            : base(options)
        {


        }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Product>()
            .HasMany(P => P.Purchases)
            .WithOne().OnDelete(DeleteBehavior.NoAction);

        }

    

        // maps to our tables in the database
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductRating> ProductRatings { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Cart> Carts{ get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<ActivationCode> ActivationCodes { get; set; }
        public DbSet<GuestCart> GuestCarts { get; set; }


    }
}
