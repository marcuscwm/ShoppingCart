using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    public class ActivationCode
    {

        public ActivationCode()
        {
            Id = new Guid();
        }

        //primary key. This is the activation code it-self
        public Guid Id { get; set; }

        //forien key to map to Purchase
        public virtual Guid PurchaseID { get; set; }

        //forien key to map to Product
        public virtual Guid ProductID { get; set; }
        

     
    }
}
