using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddMemberSystem.Classes.Util;
using System.Text.Json;

namespace AddMemberSystem.Controllers.Account
{
    public class AccountController : Controller
    {
        private readonly AppDBContext _context;

        public AccountController(AppDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var staySignedInCookie = Request.Cookies["staySignedIn"];

            if (staySignedInCookie != null)
            {
                return RedirectToAction("List", "Home");
            }

            return View("Login");
        }

        [HttpPost]
        public IActionResult Login(TB_User user, bool? StaySignedIn)
        {
            if (ModelState.IsValid)
            {
                var logInUser = _context.TB_Users.FirstOrDefault(u => u.UserID == user.UserID);

                if (logInUser != null)
                {
                    string hashedEnteredPassword = HashUtil.ComputeSHA256Hash(user.Password);
                    Console.WriteLine("hashed pwd!!: " + hashedEnteredPassword);                  

                    if (hashedEnteredPassword == logInUser.Password)
                    {
                        string userJson = JsonSerializer.Serialize(logInUser);

                        if (StaySignedIn.HasValue && StaySignedIn.Value)
                        {
                            // Set a cookie with an expiration date of 7 days
                            var options = new CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddDays(7),
                                HttpOnly = true,
                                SameSite = SameSiteMode.Lax,
                                Secure = false
                            };

                            Response.Cookies.Append("staySignedIn", "true", options);
                        }

                        HttpContext.Session.SetString("loginUser", userJson);

                        var storedUserJson = HttpContext.Session.GetString("loginUser");

                        _context.SaveChanges();

                        // Check for the "staySignedIn" cookie in the request
                        var staySignedInCookie = Request.Cookies["staySignedIn"];
                        if (!string.IsNullOrEmpty(staySignedInCookie))
                        {
                            return View("Login");
                        } else
                        {
                            return RedirectToAction("List", "Home");


                        }


                       
                    }
                    else
                    {
                        _context.SaveChanges();
                    }
                }
            }

            ModelState.AddModelError(string.Empty, "Please enter correct credentials.");
            return View("Login");
        }

        public IActionResult Logout()
        {           

            HttpContext.Session.Clear();

            return View("Login");
        }
    }
}
