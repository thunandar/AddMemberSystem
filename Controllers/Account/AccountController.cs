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
            return View("Login");
        }

        [HttpPost]
        public IActionResult Login(TB_User user)
        {
            if (ModelState.IsValid)
            {
                var logInUser = _context.TB_Users.FirstOrDefault(u => u.UserID == user.UserID);

                if (logInUser != null)
                {
                    string hashedEnteredPassword = HashUtil.ComputeSHA256Hash(user.Password);
                    Console.WriteLine("hashed pwd: " + hashedEnteredPassword);

                    if (hashedEnteredPassword == logInUser.Password)
                    {
                        string userJson = JsonSerializer.Serialize(logInUser);

                        HttpContext.Session.SetString("loginUser", userJson);

                        var storedUserJson = HttpContext.Session.GetString("loginUser");

                        _context.SaveChanges();

                        return RedirectToAction("List", "Home");
                    }
                    else
                    {
                        _context.SaveChanges();
                    }

                }

            }
            ModelState.AddModelError(string.Empty, "Please enter a correct credentials.");
            return View("Login");
        }

        public IActionResult Logout()
        {           

            HttpContext.Session.Clear();

            return View("Login");
        }
    }
}
