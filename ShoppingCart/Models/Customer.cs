using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{

    public class Customer
    {
        public Customer()
        {
            Id = new Guid();
            ShoppingCarts = new List<Cart>();
            Purchases = new List<Purchase>();
            ProductRatings = new List<ProductRating>();
            RewardPoint = 0;
        }
    
        //primary key
        public Guid Id  { get; set; }

        [Required]
        [MaxLength(25)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }

        // Hash'ed password
        [Required]
        public byte[] PassHash { get; set; }

        //Reward points earned. Can be null as well
        public int RewardPoint { get; set; }

        //Customer has 1 to many relationship with ShoppingCart
        public virtual ICollection<Cart> ShoppingCarts { get; set; }

        //Customer has 1 to many relationship with Purchase
        public virtual ICollection<Purchase> Purchases { get; set; }

        //Customer has 1 to many relationship with ProductRating 
        public virtual ICollection<ProductRating> ProductRatings { get; set; }

    }
}
