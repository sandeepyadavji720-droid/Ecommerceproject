using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Project_E_commerce.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        }
      public IActionResult AddCategory()
        {
            return View();
        }
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
          
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            
            return RedirectToAction("Login", "Home"); 
        }

        public IActionResult Product(int id)
        {
            ViewBag.CategoryId =id;
            return View();
        }
    }
}
