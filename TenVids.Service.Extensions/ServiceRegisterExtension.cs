using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository.IRepository;
using TenVids.Services;
using TenVids.Services.IServices;

namespace TenVids.Service.Extensions
{
    public class ServiceRegisterExtension : IServiceRegisterExtension
    {
        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
        
            services.AddControllersWithViews();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUnitOfWork, IUnitOfWork>();
            var connectionString = configuration.GetConnectionString("TenVidDb");
            services.AddDbContext<TenVidsApplicationContext>(options =>
                options.UseSqlServer(connectionString));

  
            services.AddIdentity<ApplicationUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<TenVidsApplicationContext>()
            .AddDefaultTokenProviders();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            services.AddAuthorization();
        }
    }

    
}