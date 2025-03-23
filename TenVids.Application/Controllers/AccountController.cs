using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TenVids.Services.IServices;

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
    }
}
