using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.Models
{
    // class to receive Jason object as product
    public class ProdJson
    {
        public ProdJson()
        {     

        }         
        public string ProductName { get; set; }     
    }
}
