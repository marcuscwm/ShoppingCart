using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Models
{
    public class GuestCart
    {
        public GuestCart (){

            Id = new Guid();
        }

        public Guid Id { get; set; }

        //forien key to map to Product
        public virtual Guid ProductId { get; set; }

        public int OrderQty { get; set; }
    }
}
