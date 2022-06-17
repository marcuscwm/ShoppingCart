using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class Cart
    {

        public Cart()
        {
            Id = new Guid();
        }

        //primary key
        public Guid Id { get; set; }

        //forien key to map to Product
        public virtual Guid ProductId { get; set; }

        //forien key to map to Customer
        public virtual Guid CustomerId { get; set; }

        public int OrderQty { get; set; }

     
    }
}
