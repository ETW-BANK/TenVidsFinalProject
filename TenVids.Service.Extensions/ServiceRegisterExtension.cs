using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Repository;
using TenVids.Repository.IRepository;
using TenVids.Services;
using TenVids.Services.HelperMethods;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.Utilities.FileHelpers;
using TenVids.Utilities.Mapper;

namespace TenVids.Service.Extensions
{
    public class ServiceRegisterExtension : IServiceRegisterExtension
    {
        public void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configBuilder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false)
             .AddJsonFile($"appsettings.{env}.json", optional: true)
             .AddEnvironmentVariables();
            var config = configBuilder.Build();

            services.AddControllersWithViews();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IVideosService, VideosService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IPicService,PicService>();
            services.AddScoped<IHelper, Helper>();
            services.AddScoped<ISideBarService, SideBarService>(); 
            services.AddScoped<IMembersService, MembersService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IHomeService,HomeService>();
           services.AddScoped<IVideoViewService, VideoViewService>();
            services.Configure<FileUploadConfig>(configuration.GetSection("FileUpload"));
            services.AddHttpContextAccessor();
            services.AddSession();
           services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
   
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