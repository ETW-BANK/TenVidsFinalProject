using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles =SD.UserRole)]
    public class ChannelController : Controller
    {
      private readonly IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService; 
        }

        public async Task<IActionResult> Index()
        {
            var model = await _channelService.GetUserChannelAsync();
            return View(model);

        }
    }
}
