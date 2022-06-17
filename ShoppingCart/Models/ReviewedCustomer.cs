using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{

    public class ReviewedCustomer
    {
        public ReviewedCustomer()
        {
     

        }
         
        public string FullName { get; set; }
        public string ReviewComment { get; set; }
        public DateTime ReviewDate { get; set; }


    }
}
