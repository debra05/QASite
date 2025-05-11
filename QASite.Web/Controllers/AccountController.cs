using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using QASite.Data;
using System;

namespace QASite.Web.Controllers
{
    public class AccountController : Controller
    {
          private string _connectionString;

            public AccountController(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("ConStr");
            }

            public IActionResult Signup()
            {
                return View();
            }

        [HttpPost]
        public IActionResult Signup(User user, string password)
        {
            var repo = new UserRepository(_connectionString);
            repo.AddUser(user, password);
            return Redirect("/Account/Login");
        }

        public IActionResult Login()
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"];
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var repo = new UserRepository(_connectionString);
            var person = repo.Login(email, password);
            if (person == null)
            {
                TempData["Message"] = "Invalid login..., try again.";
                return Redirect("/account/login");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            HttpContext.SignInAsync(new ClaimsPrincipal(
                new ClaimsIdentity(claims, "Cookies", ClaimTypes.Email, "role"))).Wait();


            return Redirect("/home/index");
        }
        public IActionResult EmailExists(string email)
        {
            var repo = new UserRepository(_connectionString);

            return Json(new
            {
                EmailExists = repo.EmailExists(email)
            });
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/account/login");
        }
    }
}

