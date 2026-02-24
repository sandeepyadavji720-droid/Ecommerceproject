using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain_layer.Model;
using Application_layer.Interface;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_E_commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly ILoginRepository _lrepo;
        private readonly ICategoryRepository _crepo;
        private readonly IProductRepository _prepo;
        private readonly IGetAllCategoryRepository _gcrepo;
        private readonly IGetCategoryWiseProduct _gcwrepo;
        public AuthApiController(IUserRepository repo, ILoginRepository lrepo, ICategoryRepository crepo, IProductRepository prepo, IGetAllCategoryRepository gcrepo, IGetCategoryWiseProduct gcwrepo)
        {
            _repo = repo;
            _lrepo = lrepo;
            _crepo = crepo;
            _prepo = prepo;
            _gcrepo = gcrepo;
            _gcwrepo = gcwrepo;
        }
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            _repo.Register(user);
            return Ok();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            // Call repository
            var res = _lrepo.Login(model);

            if (res == null || res.Count == 0)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // Get the first user from list (there should only be one)
            var user = res.First();

            // Create claims for cookie authentication
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.email),
            new Claim(ClaimTypes.Role, user.role)  // role comes from DB
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign in user with cookie authentication
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            // Return role for frontend redirect
            return Ok(new { role = user.role });
        }


        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromForm] CategoryModel category)
        {
            if (category.image == null)
                return BadRequest("Image not received");



            string fileName = category.image.FileName;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                category.image.CopyTo(stream);
            }

         
            category.imagepath = "/content/images/" + fileName;

           int res= _crepo.AddCategory(category);

            return Ok(res);
        }
       
        [HttpGet("GetCategories")]
        public IActionResult GetCategories(ProductModel model)
        {
            var res = _gcrepo.GetAllCategory(model);
            return Ok(res);
        }



        [HttpPost("AddProduct")]
        public IActionResult AddProduct([FromForm] ProductModel product)
        {
            if (product.image == null)
                return BadRequest("Image missing");

            string fileName = Guid.NewGuid() + Path.GetExtension(product.image.FileName);

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/products");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                product.image.CopyTo(stream);
            }

           
            product.imagepath = "/content/products/" + fileName;

           int res= _prepo.AddProduct(product);

            return Ok(res);
        }
        [HttpGet("GetProductsByCategory/{id}")]
        public IActionResult GetProductsByCategory(int? id)
        {
            
                var data = _gcwrepo.GetCateWiseProduct(id);
                return Ok(data);
            }
           

    }
}
