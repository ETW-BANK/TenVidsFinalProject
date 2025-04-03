using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
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
                TempData["ChannelModel"] = model; 
                TempData["error"] = "Please correct the errors below";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _channelService.CreateChannelAsync(model);
                TempData["success"] = "Channel created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["error"] = ex.Message;
                TempData["ChannelModel"] = model;
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException)
            {
                return Challenge();
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred while creating your channel";
                TempData["ChannelModel"] = model;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}