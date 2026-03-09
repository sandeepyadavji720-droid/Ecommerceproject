using Application_layer.Interface;
using Domain_layer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
        private readonly IConfiguration _configuration;
        private readonly ISingleUserRepository _irepo;

        public AuthApiController(
            IUserRepository repo,
            ILoginRepository lrepo,
            ICategoryRepository crepo,
            IProductRepository prepo,
            IGetAllCategoryRepository gcrepo,
            IGetCategoryWiseProduct gcwrepo,
            IConfiguration configuration,
            ISingleUserRepository irepo)
        {
            _repo = repo;
            _lrepo = lrepo;
            _crepo = crepo;
            _prepo = prepo;
            _gcrepo = gcrepo;
            _gcwrepo = gcwrepo;
            _configuration = configuration;
            _irepo = irepo;
        }

        // ================= USERS =================

        [Authorize(Roles = "admin")]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            var data = _repo.GetAllUsers();
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateUser")]
        public IActionResult UpdateUser([FromBody] UserModel user)
        {
            int res = _repo.UpdateUser(user);
            return Ok(res);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("DeleteUser")]
        public IActionResult DeleteUser([FromBody] string email)
        {
            int res = _repo.DeleteUser(email);
            return Ok(res);
        }

        // ================= REGISTER =================

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserModel user)
        {
            int res = _repo.Register(user);
            return Ok(res);
        }

        // ================= LOGIN =================

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var res = _lrepo.Login(model);

            if (res == null || res.Count == 0)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var user = res.First();

            var claims = new[]
            {
        new Claim(ClaimTypes.Email, user.email),
        new Claim(ClaimTypes.Role, user.role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            
            Response.Cookies.Append("jwt", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddMinutes(60)
            });

            return Ok(new
            {
                token = jwtToken,
                role = user.role,
                email = user.email
            });
        }
        // ================= CATEGORY =================

        [Authorize(Roles = "admin")]
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

            int res = _crepo.AddCategory(category);

            return Ok(res);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("DeleteCategory")]
        public IActionResult DeleteCategory([FromForm] int id)
        {
            var result = _crepo.DeleteCategory(id);

            if (result > 0)
                return Ok(new { success = true, message = "Deleted Successfully" });

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("GetCategories")]
        public IActionResult GetCategories(ProductModel model)
        {
            var res = _gcrepo.GetAllCategory(model);
            return Ok(res);
        }

        // ================= PRODUCT =================

        [Authorize(Roles = "admin")]
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

            int res = _prepo.AddProduct(product);

            return Ok(res);
        }

        [AllowAnonymous]
        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            var data = _prepo.GetProductById(id);

            if (data == null)
                return NotFound();

            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("DeleteProduct/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _prepo.DeleteProduct(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("GetProductsByCategory/{id}")]
        public IActionResult GetProductsByCategory(int? id)
        {
            var data = _gcwrepo.GetCateWiseProduct(id);
            return Ok(data);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateCategory")]
        public IActionResult UpdateCategory([FromForm] CategoryModel category)
        {
            string imagePath = category.imagepath;

            if (category.image != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(category.image.FileName);

                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    category.image.CopyTo(stream);
                }

                imagePath = "/content/images/" + fileName;
            }

            category.imagepath = imagePath;

            int res = _crepo.UpdateCategory(category);

            return Ok(res);
        }
        [Authorize(Roles = "admin")]
        [HttpPost("UpdateProduct")]
        public IActionResult UpdateProduct([FromForm] ProductModel product)
        {
            string imagePath = product.imagepath;

            if (product.image != null)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(product.image.FileName);

                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/products");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fullPath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    product.image.CopyTo(stream);
                }

                imagePath = "/content/products/" + fileName;
            }

            product.imagepath = imagePath;

            int res = _prepo.UpdateProduct(product);

            return Ok(res);
        }
        //[Authorize]
        [HttpGet("GetProfile")]
        public IActionResult GetProfile()
        {
            string email = User.FindFirst(ClaimTypes.Email)?.Value;

            var user = _irepo.GetUserByEmail(email);

            return Ok(user);
        }
        //[Authorize]
        [HttpPost("UpdateProfile")]
        public IActionResult UpdateProfile([FromBody] UserModel model)
        {
            string email = User.FindFirst(ClaimTypes.Email)?.Value;

            model.email = email;
            

            int res = _irepo.UpdateProfile(model);

            return Ok(res);
        }
        //[Authorize]
        [HttpPost("DeleteProfile")]
        public IActionResult DeleteProfile()
        {
            string email = User.FindFirst(ClaimTypes.Email)?.Value;

            int res = _irepo.DeleteUser(email);

            return Ok(res);
        }
    }
}