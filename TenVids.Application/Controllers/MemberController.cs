using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TenVids.Application.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        public IActionResult Channel(int id)
        {
            return View();
        }
    }
}
