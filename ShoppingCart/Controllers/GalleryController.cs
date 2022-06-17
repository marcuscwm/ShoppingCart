using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace ShoppingCart.Controllers
{
    public class GalleryController : Controller
    {
        private DBContext dbContext;
        private const string UPLOAD_DIR = "Images";
        public GalleryController(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string search, string sort)
        {
            //start of snippet
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
            //end of snippet of code

            if (search == null)
            {
                search = "";
            }

            List<Product> searchResult = dbContext.Products.Where(x => x.ProductName.Contains(search)).ToList();
            ViewBag.SearchResult= searchResult;
            ViewBag.SearchInput= search;

            if (sort == "asc")
            {
                ViewBag.SearchResult = searchResult.OrderBy(x => x.Price).ToList();
            }
            else if (sort == "desc")
            {
                ViewBag.SearchResult = searchResult.OrderByDescending(x => x.Price).ToList();
            }

            return View();
        }

        public IActionResult Product(Guid productId)
        {
            if (productId == null || productId == Guid.Empty)
            {
                return RedirectToAction("Index", "Gallery");
            }

            //start of snippet
                Customer currentCustomer = CheckLoggedIn();

            ViewBag.CartContents = CountNumberOfItems();
            if (currentCustomer != null)
            {
                ViewBag.CurrentUserName = currentCustomer.FullName;
            }
            else
            {
                ViewBag.CurrentUserName = "Guest User";
            }
            //end of snippet of code

            Product product = getProduct(productId);
            //retreive average rating and total count of ratings from DB
            Dictionary<string, int> productRating = getProductRating(productId);
            //Retrive List of reviews comments and respective customer from DB
            List<ReviewedCustomer> ReviewedCustomers = GetReviewComment(productId);

            //pass all retrieved info to the view

            ViewBag.currentCustomer = currentCustomer;
            ViewBag.product = product;
            ViewBag.productRating = productRating;
            ViewBag.reviewedCustomers = ReviewedCustomers;
            ViewBag.uploadDir = "../" + UPLOAD_DIR;
            return View();          
        }

        //Returns List of reviews comments and respective customer Name from DB
        //public IEnumerable<object> GetReviewComment(Guid productId)
        public List<ReviewedCustomer> GetReviewComment(Guid productId)

        {
            List<ReviewedCustomer> reviewedCustomers = new List<ReviewedCustomer>();
            var reviewComments =(from p in dbContext.ProductRatings 
                                where p.ProductId == productId && !string.IsNullOrEmpty(p.ReviewComment)
                                join c in dbContext.Customers on p.CustomerId equals c.Id
                                orderby p.ReviewTime descending
                                 select new
                                {
                                    c.FullName,
                                    p.ReviewComment,
                                    p.ReviewTime

                                })
                                .ToList();
            foreach (var item in reviewComments)
            {
                ReviewedCustomer reviewedCustomer = new ReviewedCustomer();
                reviewedCustomer.FullName = item.FullName;
                reviewedCustomer.ReviewComment = item.ReviewComment;
                reviewedCustomer.ReviewDate = 
                    new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(item.ReviewTime).ToLocalTime();
                reviewedCustomers.Add(reviewedCustomer);
            }
            return reviewedCustomers;
        }
        public Product getProduct(Guid productId)
        {

            Product product = dbContext.Products.Where(x =>
               x.Id == productId
           ).First();

            return product;
        }
        public IActionResult PassToCart([FromBody] ProdJson prodJson)
        {
            string ProductName = prodJson.ProductName;

            //add to gust cart if cutomer is not logged in

            AddToCart(ProductName);         
            
            //return ok to Jason
            return Json(new { isOkay = true });
        }
        
        public Dictionary<string, int> getProductRating(Guid productId)
        {
            bool exist = dbContext.ProductRatings.Where(x => x.ProductId == productId).Any();
            var result = new Dictionary<string, int>();
            var avgRating = 0;
            var ratingCount = 0;
            if (exist)
            {
                avgRating = Convert.ToInt32(dbContext.ProductRatings.Where(x => x.ProductId == productId).Average
                              (x => x.Rating));

                ratingCount = dbContext.ProductRatings.Where(x => x.ProductId == productId).Count();
            }           
            result.Add("average", avgRating);
            result.Add("count", ratingCount);
            return result;

        }


        //for data on top of navbar
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

        public void AddToCart(string productName)
        {
            //check for the current product in the database
            Product newProd = dbContext.Products.FirstOrDefault(x => x.ProductName == productName);

            Customer currentCustomer = CheckLoggedIn();

            if (currentCustomer != null)
            {
                Cart itemInCart = dbContext.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);

                //query in the database whether the item already exists in the shopping cart of the customer
                if (itemInCart == null)
                {
                    //if not, create a new entry
                    dbContext.Carts.Add(new Cart
                    {
                        ProductId = newProd.Id,
                        CustomerId = currentCustomer.Id,
                        OrderQty = 1
                    });
                }
                else
                {
                    //if found, add 1 to the product quantity
                    itemInCart.OrderQty++;
                }
                dbContext.SaveChanges();
            }
            else
            {

                GuestCart itemInCart = dbContext.GuestCarts.FirstOrDefault(x => x.ProductId == newProd.Id);

                //query in the database whether the item already exists in the shopping cart of the customer
                if (itemInCart == null)
                {
                    //if not, create a new entry
                    dbContext.GuestCarts.Add(new GuestCart
                    {
                        ProductId = newProd.Id,
                        OrderQty = 1
                    });
                }
                else
                {
                    //if found, add 1 to the product quantity
                    itemInCart.OrderQty++;
                }
                dbContext.SaveChanges();
            }
        }

    }
}
