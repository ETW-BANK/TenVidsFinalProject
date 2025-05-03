using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TenVids.Data.Access.Data;
using TenVids.Models;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TenVidsApplicationContext _context;
        public AccountService(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor,TenVidsApplicationContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context; 
        }
        public Task<LoginVM> Login(string returnurl)
        {

            var loginVM = new LoginVM()
            {
                RetunUrl = returnurl
            };

            return Task.FromResult(loginVM);
        }
        public async Task<bool> LoginAsync(LoginVM loginVM)
        {
            if (loginVM == null || string.IsNullOrEmpty(loginVM.UserName) || string.IsNullOrEmpty(loginVM.Password))
            {
                return false; 
            }
           
            var user = await _userManager.FindByNameAsync(loginVM.UserName);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(loginVM.UserName);
            }

           
            else if (user.LockoutEnabled)
            {
                return false;
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginVM.Password,
                isPersistent: false, 
                lockoutOnFailure: false); 

            if (result.Succeeded)
            {
               
                await LoginHandlerAsync(user);
                return true; 
            }

            return false; 
        }
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        public async Task Register(RegisterVM registerVM)
        {
           if(!registerVM.Password.Equals(registerVM.ConfirmPassword))
            {
                throw new Exception("Password and Confirm Password must match");
            }
           if(await EmailExists(registerVM.Email))
            {
                throw new Exception($"Email '{registerVM.Email}' already exists");
            }
            if (await NameExists(registerVM.Name))
            {
                throw new Exception($"User '{registerVM.Name}' already exists");
            }

            var user = new ApplicationUser()
            {
                Name = registerVM.Name,
                Email = registerVM.Email,
                UserName = registerVM.Email
            };

            var result = await _userManager.CreateAsync(user,registerVM.Password);
            await _userManager.AddToRoleAsync(user, SD.UserRole);

            if (!result.Succeeded)
            {
                throw new Exception("User creation failed");
            }

            await LoginHandlerAsync(user);

        }

        #region Private Methods
        private async Task LoginHandlerAsync(ApplicationUser user)
        {
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName!));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email!));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var userChannelId = await _context.Channels
                .Where(x => x.AppUserId == user.Id)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync();

            if (userChannelId.HasValue)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, userChannelId.Value.ToString()));
            }

            // Add user roles to claims
            var roles = await _userManager.GetRolesAsync(user);
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var principal = new ClaimsPrincipal(claimsIdentity);

            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        }


        private async Task<bool> EmailExists(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user != null;
        }

        private async Task<bool> NameExists(string name)
        {
            var user = await _userManager.FindByNameAsync(name);
            return user != null;
        }
        #endregion
    }
}
