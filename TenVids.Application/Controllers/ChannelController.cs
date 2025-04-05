using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;


namespace TenVids.Application.Controllers
{
    [Authorize(Roles = SD.UserRole)]
    public class ChannelController : Controller
    {
        private readonly IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        public async Task<IActionResult> Index()
        {
            var channel = await _channelService.GetUserChannelAsync();
   
            if (channel == null)
            {
                
                var model = TempData["ChannelModel"] as ChannelAddEditVM ?? new ChannelAddEditVM();
                return View(model);
            }
         
            return View(channel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChannel(ChannelAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
              
                return RedirectToAction(nameof(Index));
            }

            var result = await _channelService.CreateChannelAsync(model);

            if (result.Succeeded)
            {
                TempData["success"] = "Channel created successfully!";
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = result.ErrorMessage;
            TempData["ChannelModel"] = JsonSerializer.Serialize(model);
            return RedirectToAction(nameof(Index));
        }
    }
}