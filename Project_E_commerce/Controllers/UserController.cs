using Application_layer.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Project_E_commerce.Controllers
{
    [Authorize(Roles = "user")]
    public class UserController : Controller
    {
        private readonly ISingleUserRepository _userService;
        public UserController(ISingleUserRepository singleUser)
        { 
            _userService = singleUser;
        }
        public IActionResult Profile()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Login", "Account");

            var userProfile = _userService.GetSingleUser(email);

            if (userProfile == null)
                return NotFound("User not found");

            return View(userProfile);
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Category()
        {
          
            return View();
        }
        public IActionResult products(int id)
        {
            ViewBag.CategoryId = id;
            return View();
        }
       
        [HttpGet]
        public async Task<IActionResult> Logout()
        {

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return RedirectToAction("Login", "Home");
        }
    }
}
