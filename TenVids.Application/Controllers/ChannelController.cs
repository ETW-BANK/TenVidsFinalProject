using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenVids.Utilities;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles =SD.UserRole)]
    public class ChannelController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
