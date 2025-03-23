using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public AccountService(UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public Task<LoginVM> Login(string returnurl)
        {

            var loginVM = new LoginVM()
            {
                RetunUrl = returnurl
            };

            return Task.FromResult(loginVM);
        }
    }
}
