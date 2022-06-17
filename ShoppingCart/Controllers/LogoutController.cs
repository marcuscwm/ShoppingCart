using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Models;
using Microsoft.EntityFrameworkCore;

namespace ShoppingCart.Controllers
{
    public class LogoutController : Controller
    {
        private DBContext dbContext;

        public LogoutController(DBContext dbContext)
        {
            
            this.dbContext = dbContext;
           
        }
        public IActionResult Index()
        {// remove session from our database
            if (Request.Cookies["SessionId"] != null)
            {//session Id is converted to string as our Session controller has PK in string type//
                //string sessionId = System.Guid.NewGuid().ToString();//
              Guid sessionId = Guid.Parse(Request.Cookies["sessionId"]);


                Session session = dbContext.Sessions.FirstOrDefault(x => x.Id == sessionId);
                if (session != null)
                {
                    // delete session record from our database;
                    dbContext.Remove(session);

                    // commit to save changes
                    dbContext.SaveChanges();
                }
            }

            // ask client to remove these cookies so that
            // they won't be sent over next time
            Response.Cookies.Delete("SessionId");
            Response.Cookies.Delete("Username");

            return RedirectToAction("Index", "Login");
        }
        
        
    }
}
