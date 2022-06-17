using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using System.Security.Cryptography;
using System.Text;


namespace ShoppingCart
{
    public class DB
    {


        private DBContext dbContext;

        public DB(DBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Seed()
        {
            SeedCustomer();
            SeedProduct();
            SeedProductRating();
                      
        }


        public void SeedCustomer()
        {
            // get a hash algorithm object
            HashAlgorithm sha = SHA256.Create();

            string[] usernames = { "Tom_Cruise", "Emma_Watson", "Brad_Pitt", "Daniel_Craig", "Emma_Stone", "Al_Pacino" };
            string[] fullnames = { "Tom Cruise", "Emma Watson", "Brad Pitt", "Daniel Craig", "Emma Stone", "Al Pacino" };
            // as our system's default, new users have their 
            // passwords set as "secret"
            string[] password = { "secret01", "secret02", "secret03", "secret04", "secret05", "secret06" };

            for (int i = 0; i < usernames.Length; i++)
            {
                // we are concatenating (i.e. username + password) to generate
                // a password hash to store in the database
                string combo = usernames[i] + password[i];
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(combo));

                // add customers
                dbContext.Add(new Customer
                {
                    UserName = usernames[i],
                    FullName = fullnames[i],
                    PassHash = hash,
                    RewardPoint = 0
                }); ;

                // commit our changes in the database
                dbContext.SaveChanges();
            }
        }

        private void SeedProduct()
        {
            dbContext.Add(new Product
            {

                ProductName = ".NET Charts",
                Price = 99.00,
                Description = "Brings powerful charting capabilities to your .NET applications.",
                ImageName = "dotNETCharts.jpg"

            });

            dbContext.Add(new Product
            {

                ProductName = ".NET Paypal",
                Price = 69.00,
                Description = "Integrate your .NET apps with paypal the easy way!",
                ImageName = "dotNETPaypal.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET ML",
                Price = 299.00,
                Description = "Supercharged .NET machine learning libraries.",
                ImageName = "dotNETML.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Analytics",
                Price = 299.00,
                Description = "Performs data mining and analytics easily in .NET.",
                ImageName = "dotNETAnalytics.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Logger",
                Price = 299.00,
                Description = "Logs and aggregates events easily in your .NET apps.",
                ImageName = "dotNETLogger.jpg"
            });

            dbContext.Add(new Product
            {
                ProductName = ".NET Numerics",
                Price = 199.00,
                Description = "Powerful numerical method for your .NET simulations.",
                ImageName = "dotNETNumerics.jpg"
            });

            dbContext.SaveChanges();

        }
        private void SeedProductRating()
        {
            string[] usernames = { "Tom_Cruise", "Emma_Watson", "Brad_Pitt", "Daniel_Craig", "Emma_Stone", "Al_Pacino" };
            string[] productName = { ".NET Charts",".NET Paypal", ".NET ML", ".NET Analytics", ".NET Logger", ".NET Numerics" };
            string[] ReviewComment = { "Awesome. This was what I was looking for",
                "Not that good as I expected", "It would be nice to have more feature",
                "Fuctionally ok, but not very user friendly", "I don't recommend it", "Satisfied" };
            

            List<Customer> customers = new List<Customer>();
            List<Product> products = new List<Product>();
            
                foreach (var usrnm in usernames)
                {
                    Customer customer = dbContext.Customers.FirstOrDefault(x =>
                       x.UserName == usrnm
                       );
                    foreach (var prdnm in productName)
                    {
                        Product product = dbContext.Products.FirstOrDefault(x =>
                        x.ProductName == prdnm
                    );
                        if (customer != null && product != null)
                        {
                            Random rnd = new Random();
                            int num = rnd.Next(1, 6);
                            ProductRating productRating1 = new ProductRating
                            {                                
                                Rating = num,
                                ReviewComment = ReviewComment[num]
                            };
                            customer.ProductRatings.Add(productRating1);
                            product.ProductRatings.Add(productRating1);
                        }
                        dbContext.SaveChanges();
                    }
                }            
        }
                

    }
}


        
    

