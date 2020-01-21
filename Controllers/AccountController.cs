using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PatientZero.Models;
using System.Net.Mail;
using System.Net;

namespace PatientZero.Controllers
{
    public class AccountController : Controller {
        
        [HttpGet]
        public ActionResult Registration() {
            return View();
        }

        [HttpPost]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified, ActivationCode, Salt")] User user) {

            if (ModelState.IsValid) {
                if(IsEmailExist(user.EmailAddress)) {
                    ModelState.AddModelError("EmailAddress", "Email already exist");
                    return View(user);
                }

                user.ActivationCode = Guid.NewGuid();
                user.Salt = Guid.NewGuid();
                user.Password = Crypto.Hash(user.Password, user.Salt);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword, user.Salt);
                user.IsEmailVerified = false;

                using (DatabaseEntities db = new DatabaseEntities()) {
                    db.Users.Add(user);
                    db.SaveChanges();
                }

                //SendVerificationLinkEmail(user.EmailAddress, user.ActivationCode.ToString(), user.FirstName, user.LastName);
                return RedirectToAction("Home", "Index");

            } else {
                ViewBag.errorMessage = "Invalid request";
                return View(user);
            }
        }

        public ActionResult VerifyAccount(string id) {
            using (DatabaseEntities db = new DatabaseEntities()) {
                db.Configuration.ValidateOnSaveEnabled = false;
                var result = db.Users
                    .Where(a => a.ActivationCode == new Guid(id))
                    .FirstOrDefault();
                if(result != null) {
                    result.IsEmailVerified = true;
                    db.SaveChanges();
                    ViewBag.Status = true;
                } else {
                    ViewBag.Status = false;
                }
            }
            return View();
        }

        [NonAction]
        private bool IsEmailExist(string emailAddress) {
            using (DatabaseEntities db = new DatabaseEntities()) {
                return db.Users.Where(a => a.EmailAddress == emailAddress).FirstOrDefault() != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailAddress, string activationCode, 
            string firstName, string lastName) {

            var verifyUrl = "/Account/VerifyAccount/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("akusoul.ar@gmail.com", "Patient Zero");
            var toEmail = new MailAddress(emailAddress, firstName + " " + lastName);
            var subject = "Your account has been successfully created!";
            var body = "hello<strong> " + firstName + ",</strong><br/>" +
                "We are excited to tell you that your account has been created. " +
                "Please click on the link below to verify your account. <br/><br/> " +
                "<a href = '" + link + "'>" + link + "</a>";
            using (var smtp = new SmtpClient())
            using (var message = new MailMessage(fromEmail, toEmail) {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            smtp.Send(message);
        }
    }
}