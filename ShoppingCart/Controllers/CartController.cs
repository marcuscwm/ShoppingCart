using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly DBContext db;

        public CartController(DBContext dbContext)
        {

            this.db = dbContext;

        }

        public IActionResult Index()
        {

            Customer currentCustomer = CheckLoggedIn();

            Dictionary<Product, int> countOfItems = new Dictionary<Product, int>();
            List<Cart> customerCart = new List<Cart>();
            List<GuestCart> guestCart = new List<GuestCart>();
            //Seed an item for user
            if (currentCustomer != null)
            {

                Product newProd = FetchRandomProduct();

                db.SaveChanges();

                customerCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

                foreach (Cart item in customerCart)
                {
                    Product newItem = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    countOfItems.Add(newItem, item.OrderQty);
                };

                ViewBag.CurrentUserName = currentCustomer.FullName;

            }
            else
            {

                Product newProd = FetchRandomProduct();

                db.SaveChanges();

                guestCart = db.GuestCarts.ToList();

                foreach (GuestCart item in guestCart)
                {
                    Product newItem = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    countOfItems.Add(newItem, item.OrderQty);
                };
                ViewBag.CurrentUserName = "Guest User";

            }

            //initializing

            ViewBag.CurrentCustomer = currentCustomer;
            ViewBag.CountOfItems = countOfItems;
            ViewBag.TotalPrice = CalculateTotalPrice();
            ViewBag.CartContents = CountNumberOfItems();
            return View();
        }
        //remove item from cart using JSON
        public IActionResult RemoveItem([FromBody] ProdJson prodJson)
        {
            string ProductName = prodJson.ProductName;            
            SubtractFromCart(ProductName);
            return Json(new { isOkay = true });

        }
        //Add item to cart using JSON
        public IActionResult AddItem([FromBody] ProdJson prodJson)
        {
            string ProductName = prodJson.ProductName;            
            AddToCart(ProductName);
            return Json(new { isOkay = true });
        }

        public IActionResult RemoveProduct([FromBody] ProdJson prodJson)
        {
            string ProductName = prodJson.ProductName;
            RemoveProdctFromCart(ProductName);
            return Json(new { isOkay = true });
        }

        //AddToCart (will be used in the gallery, the cart page and the results page)
        public void AddToCart(string productName)
        {
            //check for the current product in the database
            Product newProd = db.Products.FirstOrDefault(x => x.ProductName == productName);

            Customer currentCustomer = CheckLoggedIn();            
            if (currentCustomer != null)
            {
                Cart itemInCart = db.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);

                //query in the database whether the item already exists in the shopping cart of the customer
                if (itemInCart == null)
                {
                    //if not, create a new entry
                    db.Carts.Add(new Cart
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
                db.SaveChanges();
            }
            else {

                GuestCart itemInCart = db.GuestCarts.FirstOrDefault(x => x.ProductId == newProd.Id);

                //query in the database whether the item already exists in the shopping cart of the customer
                if (itemInCart == null)
                {
                    //if not, create a new entry
                    db.GuestCarts.Add(new GuestCart
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
                db.SaveChanges();
            }
        }

        //remove the entore product from the cart
        public void RemoveProdctFromCart(string productName)
        {
            Product newProd = db.Products.FirstOrDefault(x => x.ProductName == productName);

            Customer currentCustomer = CheckLoggedIn();

            if (currentCustomer != null)
            {
                Cart itemInCart = db.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);
                                
                    db.Carts.Remove(itemInCart);                
                db.SaveChanges();
            }
            else
            {
                GuestCart itemInCart = db.GuestCarts.FirstOrDefault(x => x.ProductId == newProd.Id);
                                
                    db.GuestCarts.Remove(itemInCart);
                
                db.SaveChanges();
            }
        }

        //will be used in the cart page
        public void SubtractFromCart(string productName)
        {
            Product newProd = db.Products.FirstOrDefault(x => x.ProductName == productName);

            Customer currentCustomer = CheckLoggedIn();

            if (currentCustomer != null)
            {
                Cart itemInCart = db.Carts.FirstOrDefault(x => x.ProductId == newProd.Id && x.CustomerId == currentCustomer.Id);

                if (itemInCart.OrderQty == 1)
                {
                    db.Carts.Remove(itemInCart);
                }
                else
                {
                    itemInCart.OrderQty--;
                }
                db.SaveChanges();
            }
            else
            {

                GuestCart itemInCart = db.GuestCarts.FirstOrDefault(x => x.ProductId == newProd.Id);

                if (itemInCart.OrderQty == 1)
                {
                    db.GuestCarts.Remove(itemInCart);
                }
                else
                {
                    itemInCart.OrderQty--;
                }
                db.SaveChanges();
            }
        }
        public IActionResult Checkout()
        {
            Customer currentCustomer = CheckLoggedIn();

            if (currentCustomer != null)
            {
                //get all items in the cart under the customer
                List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

                //get all current purchases of customer
                //for display later in the checkout
                Dictionary<Product, int> itemQty = new Dictionary<Product, int>();
                
                //take the price of the entire list
                double totalPrice = CalculateTotalPrice();
                ViewBag.TotalPrice = totalPrice;

                //transform all items into purchases
                foreach (Cart item in itemsInCart)
                {
                    Purchase onHand = db.Purchases.FirstOrDefault(x => x.CustomerId == currentCustomer.Id && x.ProductId == item.ProductId);

                    if (onHand == null)
                    {
                        //if it is not in the purchases db, create new purchase, add activation code and set quantity to 1
                        onHand = new Purchase
                        {
                            ProductId = item.ProductId,
                            CustomerId = currentCustomer.Id,
                            PurchaseDate = DateTime.Now,
                            PurchaseQty = item.OrderQty
                        };
                        db.Purchases.Add(onHand);
                    }
                    else
                    {
                        //if it is, add to purchase qty
                        onHand.PurchaseQty += item.OrderQty;
                    }

                    db.SaveChanges();

                    for (int i = item.OrderQty; i > 0; i--)
                    {

                        //generate activation code
                        ActivationCode newCode = new ActivationCode
                        {
                            //put the attributes
                            ProductID = item.ProductId,
                            PurchaseID = onHand.Id
                        };

                        db.ActivationCodes.Add(newCode);

                        db.SaveChanges();
                    }

                    //remove from the ShoppingCarts database
                    Cart findTheItem = db.Carts.FirstOrDefault(x => x.Id == item.Id);
                    Product findProduct = db.Products.FirstOrDefault(x => x.Id == findTheItem.ProductId);
                    itemQty.Add(findProduct, findTheItem.OrderQty);

                    db.Carts.Remove(findTheItem);
                    db.SaveChanges();
                }

                int countOfPurchases = db.Purchases.Where(x => x.CustomerId == currentCustomer.Id).ToList().Count;

                int additionalPoints = (int)Math.Floor(totalPrice / 10);
                currentCustomer.RewardPoint += additionalPoints;
                db.SaveChanges();

                //send the details of the purchases to ViewBag (for View purposes)
                //add product details to the dictionary
                ViewBag.CurrentCustomer = currentCustomer;
                ViewBag.ItemQty = itemQty;
                ViewBag.ListOfRandomProds = RecommendProducts();
               
                ViewBag.Reward = currentCustomer.RewardPoint;
               
                ViewBag.AdditionalPoint = additionalPoints;
                //all IActionResult Methods will return this
                ViewBag.CartContents = CountNumberOfItems();
                ViewBag.CurrentUserName = currentCustomer.FullName;
                //send customer details to display

                return View();
            }
            else {
                ViewBag.ComingFromCheckout = true;
                db.SaveChanges();
                //if not logged in
                return RedirectToAction("Index", "Login");
            }
        }

        public double CalculateTotalPrice() {
            double finalPrice = 0.0;

            Customer currentCustomer = CheckLoggedIn();

            if (currentCustomer != null)
            {
                //get all items in the cart under the customer
                List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

                //take the price of each item
                foreach (Cart item in itemsInCart)
                {
                    Product takePrice = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    //get the product price and multiply the quantity
                    double priceToAdd = takePrice.Price * item.OrderQty;
                    /*
                    for any discount logic concerning the price, put them here
                    */
                    finalPrice += priceToAdd;
                }
            }
            else {
                //get all items in the cart under the customer
                List<GuestCart> itemsInCart = db.GuestCarts.ToList();

                //take the price of each item
                foreach (GuestCart item in itemsInCart)
                {
                    Product takePrice = db.Products.FirstOrDefault(x => x.Id == item.ProductId);
                    //get the product price and multiply the quantity
                    double priceToAdd = takePrice.Price * item.OrderQty;

                    finalPrice += priceToAdd;
                }
            }

            return finalPrice;
        }
        //count number of items
        public int CountNumberOfItems() {
            int finalCount = 0;

            Customer currentCustomer = CheckLoggedIn();
            if (currentCustomer != null)
            {
                //get all items in the cart under the customer
                List<Cart> itemsInCart = db.Carts.Where(x => x.CustomerId == currentCustomer.Id).ToList();

                //take the quantity of each item
                foreach (Cart item in itemsInCart)
                {
                    finalCount += item.OrderQty;
                }
            }
            else
            {
                //get all items in the cart under the customer
                List<GuestCart> itemsInCart = db.GuestCarts.ToList();

                //take the quantity of each item
                foreach (GuestCart item in itemsInCart)
                {
                    finalCount += item.OrderQty;
                }
            }
            return finalCount;
        }
        //compute how many reward points for the customer is needed
        public int ComputeRewardPoints(double price) {
            int rewardsTotal = (int)price / 10;
            return rewardsTotal;
        }
        public List<Product> RecommendProducts()
        {
            List<Product> listOfProducts = new List<Product>();
            while (listOfProducts.Count < 3)
            {
                Product randomProduct = FetchRandomProduct();
                if (!listOfProducts.Contains(randomProduct))
                {
                    listOfProducts.Add(randomProduct);
                }
            }
            return listOfProducts;
        }

        public Product FetchRandomProduct()
        {
            List<Product> listOfProducts = db.Products.ToList();
            Random rnd = new Random();

            Product randomProduct = listOfProducts[rnd.Next(listOfProducts.Count())];
            return randomProduct;
        }

        public Customer CheckLoggedIn() {
            Customer currentCustomer = new Customer();
            if (Request.Cookies["SessionId"] != null)
            {
                Guid sessionId = Guid.Parse(Request.Cookies["SessionId"]);
                Session session = db.Sessions.FirstOrDefault(x => x.Id == sessionId);

                if (session == null)
                {
                    currentCustomer = null;
                    return currentCustomer;
                }

                currentCustomer = db.Customers.FirstOrDefault(x => x == session.Customer);

            }
            else
            {
                currentCustomer = null;
            }
            return currentCustomer;
        }

        public IActionResult PassToCart([FromBody] ProdJson prodJson)
        {
            string ProductName = prodJson.ProductName;

            AddToCart(ProductName);

            return Json(new { isOkay = true });
        }
    }
}