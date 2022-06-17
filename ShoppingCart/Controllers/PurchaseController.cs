using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ShoppingCart.Controllers{
    public class PurchaseController : Controller
    {
        private DBContext dbContext;
        private const string UPLOAD_DIR = "Images";
        public PurchaseController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        
        public IActionResult Index()
        {
            // get the customId via session
            Customer currentCustomer = CheckLoggedIn();
            //Redirect to login if customer try to access purchases without login in
            if (currentCustomer == null) {
                return RedirectToAction("Index", "Login");
            }

            // get purchase list via customid
            List<Purchase> purchases = dbContext.Purchases.Where(item => item.CustomerId.Equals(currentCustomer.Id)).ToList();

            Dictionary<Guid, Product> maps = new Dictionary<Guid, Product>();
            Dictionary<Guid, ActivationCode[]> activeCodeMap = new Dictionary<Guid, ActivationCode[]>();

            purchases.ForEach(purchase => {
                Product find = dbContext.Products.Where(item => item.Id.Equals(purchase.ProductId)).ToList().FirstOrDefault();

                ActivationCode[] codes = dbContext.ActivationCodes.Where(item => item.PurchaseID.Equals(purchase.Id)).ToArray();
                maps.Add(purchase.Id, find);
                activeCodeMap.Add(purchase.Id, codes);
            });

            // inject list to the view
            ViewData["purchaseList"] = purchases;
            ViewData["productMaps"] = maps;
            ViewData["activeCodeMap"] = activeCodeMap;

            //show items in layout - Gab
            ViewBag.CartContents = CountNumberOfItems();
            ViewBag.CurrentUserName = currentCustomer.FullName;
            //end of snippet of code - Gab
            return View();
        }

        public IActionResult Review(string prodSclicked)
        {
            /*Redirect to home if the user trying access by directty typing the url without selecting a
            product from purchases*/
            if (prodSclicked == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //Check if the customer has logged in and set current customer
            Customer currentCustomer = new Customer();
            currentCustomer = CheckLoggedIn();

            ViewBag.CartContents = CountNumberOfItems();
            if (currentCustomer != null)
            {
                ViewBag.CurrentUserName = currentCustomer.FullName;
            }
            else
            {
                ViewBag.CurrentUserName = "Guest User";
            }
                                  

            Product product = getProduct(prodSclicked);

            if (product == null)
            {
                return RedirectToAction("Index", "Gallery");
            }
            ViewBag.product = product;
            ViewBag.uploadDir = "../" + UPLOAD_DIR;
            return View();

        }
        public Product getProduct(string productName)
        {

            Product product = dbContext.Products.Where(x =>
                x.ProductName == productName
            ).First();

            return product;
        }

        //TO Update DB with user rating and review comment
        public IActionResult CaptureReview(IFormCollection form)
        {
           int rating = Convert.ToInt32(form["ratingValue"]);
            string comment = form["reviewComment"];
            Guid productID = Guid.Parse(form["ProductID"]);
            
            Customer currentCustomer = CheckLoggedIn();                    
            Product product = dbContext.Products.FirstOrDefault(x =>
                x.Id == productID
            );  
           
            //update DB
                if (currentCustomer != null && product != null)
                {
                    ProductRating productRatingNew = new ProductRating
                    {
                        Rating = rating,
                        ReviewComment = comment
                    };
                    currentCustomer.ProductRatings.Add(productRatingNew);
                    product.ProductRatings.Add(productRatingNew);
                }
                dbContext.SaveChanges();                                          
                return RedirectToAction("Index", "Purchase");
        }
        //Check if the rating for a already product exixt in the DB by a customer
        public bool CheckProductRatingExists(Guid ProductID, Guid CustomerID)
        {
            ProductRating productRating = dbContext.ProductRatings.FirstOrDefault(x =>
               x.ProductId == ProductID && x.CustomerId == CustomerID);
            if (productRating == null)
            {
                return false;
            }
            return true;        }



        //for data on top of navbar. don't delete - Gab
        public Customer CheckLoggedIn()
        {
            Customer currentCustomer = new Customer();

            if (Request.Cookies["SessionId"] != null)
            {
                Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);
                Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == sessionId);

                if (session == null)
                {
                    currentCustomer = null;
                    return currentCustomer;
                }
                currentCustomer = dbContext.Customers.FirstOrDefault(x => x == session.Customer);
            }
            else
            {
                currentCustomer = null;
            }
            return currentCustomer;
        }

        public int CountNumberOfItems()
        {
            int finalCount = 0;

            Customer currentCustomer = CheckLoggedIn();

            if (currentCustomer != null)
            {
                //get all items in the cart under the customer
                List<Cart> itemsInCart = dbContext.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

                //take the quantity of each item
                foreach (Cart item in itemsInCart)
                {
                    finalCount += item.OrderQty;
                }
            }
            else
            {
                //get all items in the cart under the customer
                List<GuestCart> itemsInCart = dbContext.GuestCarts.ToList();

                //take the quantity of each item
                foreach (GuestCart item in itemsInCart)
                {
                    finalCount += item.OrderQty;
                }
            }

            return finalCount;
        }
        //for data on top of navbar. don't delete - Gab
    }
}
