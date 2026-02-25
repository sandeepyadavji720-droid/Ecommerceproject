using Application_layer.Interface;
using Domain_layer.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
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
        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            var data = _prepo.GetProductById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }


        [HttpPost("UpdateProduct")]
        public IActionResult UpdateProduct([FromForm] ProductModel model)
        {
           
           
            if (model.image != null)
            {
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/products");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.image.FileName);
                string fullPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    model.image.CopyTo(stream);
                }

               
                model.imagepath = "/content/products/" + fileName;
            }
            else if (!string.IsNullOrEmpty(model.imagepath))
            {
               
                model.imagepath = model.imagepath;
            }

           
            _prepo.UpdateProduct(model);

            return Ok();
        }


        [HttpPost("DeleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _prepo.DeleteProduct(id);
            return Ok();
        }





        [HttpPost("UpdateCategory")]
        public IActionResult UpdateCategory([FromForm] CategoryModel model)
        {
            if (model.image == null)
                return BadRequest("Image not received");



            string fileName = model.image.FileName;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                model.image.CopyTo(stream);
            }


            model.imagepath = "/content/images/" + fileName;

        

        var result = _crepo.UpdateCategory(model);

            return Ok();
        }


        [HttpPost("DeleteCategory")]
        public IActionResult DeleteCategory([FromForm]int id)
        {
            var result = _crepo.DeleteCategory(id);

            if (result > 0)
                return Ok(new { success = true, message = "Deleted Successfully" });

            return BadRequest();
        }





        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserModel user)
        {
          int res= _repo.Register(user);
            return Ok(res);
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
