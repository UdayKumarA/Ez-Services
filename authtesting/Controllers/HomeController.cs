using authtesting.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace authtesting.Controllers
{
    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
          

            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public string get(UserLogin objuser)
        {
            Registration objregistration = new Registration();
            string result = objregistration.UserLogin(objuser);
            return result;
        }
        public ActionResult Redirectafterlogin()
        {
            //var username = TempData["username"];
            return View();
        }
    }
}
