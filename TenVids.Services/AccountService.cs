using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TenVids.Models;
using TenVids.Services.IServices;
using TenVids.ViewModels;

namespace TenVids.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountService(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
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

            if (user == null)
            {
                Console.WriteLine("User not found.");
                return false; 
            }

          
            var result = await _signInManager.PasswordSignInAsync(
                user,
                loginVM.Password,
                isPersistent: false, 
                lockoutOnFailure: false); 

            if (result.Succeeded)
            {
                
                Console.WriteLine("Login successful.");
               
                await LoginHandlerAsync(user);
                return true;
            }

          
            Console.WriteLine("Login failed. Reason: " + result.ToString());
            return false; 
        }
        private async Task LoginHandlerAsync(ApplicationUser user)
        {
            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

           
            var roles = await _userManager.GetRolesAsync(user);
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var principal = new ClaimsPrincipal(claimsIdentity);

            Console.WriteLine("Claims created: " + string.Join(", ", claimsIdentity.Claims.Select(c => c.Type + ": " + c.Value)));

          
            await _httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);
        }
    }
}
