using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TenVids.Services.IServices;
using TenVids.Utilities;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles = SD.UserRole)]
    public class MemberController : Controller
    {
        private readonly IMembersService _members;
        private readonly IChannelService _channelService;
        private readonly IVideosService _videoservice;
        public MemberController(IMembersService members,IChannelService channel,IVideosService videoservice)
        {
            _members = members;
            _channelService = channel;
            _videoservice = videoservice;   
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
        [HttpPost]
        public async Task<IActionResult> SubscribeChannel(int channelId)
        {
            var rsult=await _channelService.Subscribe(channelId);
            if (rsult != null)
            {

                return RedirectToAction("Channel", new { id = channelId });
            }
            TempData["notification"] = "false;Not Found; Requested channel was not found";
            return RedirectToAction("Index", "Home");
        }

        #region API CALL

        [HttpGet]
        public async Task<IActionResult> GetMemberChannelVideos(int channelId)
        {
            var channelVideos=await _videoservice.GetVideosByChannelIdAsync(channelId);

            return Json(new ApiResponse(200, result: channelVideos));
        }

        #endregion

    }
}
