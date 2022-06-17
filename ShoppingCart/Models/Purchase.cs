using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class Purchase
    {
        public Purchase()
        {
            Id = new Guid();
            ActivationCodes = new List<ActivationCode>();
        }
        //Primary Key
        public Guid Id { get; set; }

        public int PurchaseQty { get; set; }

        [Required]
        public DateTime PurchaseDate { get; set; }

        //forien key to map to Product
        public virtual Guid ProductId { get; set; }

        //forien key to map to Customer
        public virtual Guid CustomerId { get; set; }

        //Purchase has 1 to many relationship with ActivationCode 
        public virtual ICollection<ActivationCode> ActivationCodes { get; set; }
          
    }
}
