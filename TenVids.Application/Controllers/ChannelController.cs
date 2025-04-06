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

            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = result.Message;
            TempData["ChannelModel"] = JsonSerializer.Serialize(model);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateChannel(ChannelAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Invalid channel data.";
                TempData["ChannelModel"] = JsonSerializer.Serialize(model);
                return RedirectToAction(nameof(Index));
            }

            var result = await _channelService.UpdateChannelAsync(model);

            if (result.IsSuccess)
            {
                TempData["success"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["error"] = result.Message;
            TempData["ChannelModel"] = JsonSerializer.Serialize(model);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var channel = _channelService.GetChannelByIdAsync(id.Value).Result; 
            if (channel == null) return NotFound();

            return View(channel);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null || id == 0)
            {
                TempData["error"] = "Channel not found.";
                return RedirectToAction(nameof(Index));
            }

            var channel = _channelService.GetChannelByIdAsync(id.Value);

            if (channel == null)
            {
                TempData["error"] = "Channel not found.";
                return RedirectToAction(nameof(Index));
            }
            await _channelService.DeleteChannelAsync(channel.Result);
            TempData["success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}