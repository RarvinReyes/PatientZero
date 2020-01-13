using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatientZero.Models;

namespace PatientZero.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login() {

            var user = new User();
            return View(user);
        }

        public ActionResult Authenticate() {
            HttpCookie cookie = new HttpCookie("username");

            cookie.Value = "hey";

            Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Home");
        }
    }
}