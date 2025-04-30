using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TenVids.Services.IServices;

namespace TenVids.Application.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly IMembersService _members;

        public MemberController(IMembersService members)
        {
            _members = members;
        }
        public  async Task<IActionResult> Channel(int id)
        {
            var result= await _members.GetmembersChannel(id);

            if (result != null)
            {
                return View(result);    
            }
            TempData["error"] = "Channel Not Found";
            return RedirectToAction("Index","Home");
        }
    }
}
