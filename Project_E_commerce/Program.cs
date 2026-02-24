using Application_layer.Interface;
using Infrastructure_layer.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Project_E_commerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Add your repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IGetAllCategoryRepository, GetAllCategoryRepository>();
            builder.Services.AddScoped<IGetCategoryWiseProduct, GetCatWiseProductRepository>();
            builder.Services.AddScoped<ISingleUserRepository, SingleUserRepository>();

            // Add Session support (optional but useful)
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add Cookie Authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Login";          
                    options.AccessDeniedPath = "/home/login"; 
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });
            // Needed for role-based [Authorize]
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();           // Session must come before authentication
            app.UseAuthentication();    // Enable authentication
            app.UseAuthorization();     // Enable role-based authorization

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}