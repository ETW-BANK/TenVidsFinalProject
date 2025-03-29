using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TenVids.Services.IServices;
using TenVids.ViewModels;

namespace TenVids.Application.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnurl=null)
        {
          var url= await _accountService.Login(returnurl);

            return View(url);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _accountService.LoginAsync(loginVM);
                if (loginResult)
                {
                    return RedirectToAction("Index", "Home"); 
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
           
            return View(loginVM);
        }

        [HttpGet]
        public  IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View(registerVM);
            }
            try
            {
                await _accountService.Register(registerVM);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {  
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(registerVM);
            }
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (ModelState.IsValid)
            {
                await _accountService.LogoutAsync();
            }
            return RedirectToAction("Index","Home");
        }
    }
}
