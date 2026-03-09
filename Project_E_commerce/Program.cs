using Application_layer.Interface;
using Infrastructure_layer.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Project_E_commerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services
            builder.Services.AddControllersWithViews();

            // Repositories
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IGetAllCategoryRepository, GetAllCategoryRepository>();
            builder.Services.AddScoped<IGetCategoryWiseProduct, GetCatWiseProductRepository>();
            builder.Services.AddScoped<ISingleUserRepository, SingleUserRepository>();

            // CORS for Angular
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            // JWT Authentication
            var jwtKey = builder.Configuration["Jwt:Key"];

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey))
                    };

                    // ?? THIS PART IS IMPORTANT (READ TOKEN FROM COOKIE)
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var token = context.Request.Cookies["jwt"];

                            if (!string.IsNullOrEmpty(token))
                            {
                                context.Token = token;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            // Authorization
            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors("AllowAngular");

            app.UseAuthentication();   // JWT Verify

            app.UseAuthorization();    // Role Verify

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}