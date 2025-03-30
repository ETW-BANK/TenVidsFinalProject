using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
                return RedirectToAction("CreateChannel");
            }
            return View(channel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChannel(ChannelAddEditVM model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                await _channelService.CreateChannelAsync(model);
                TempData["success"] = "Channel created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                model.Name = "";
                model.Description = "";
                return View("Index", model);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["error"]= "User not authenticated";
                return Challenge(); 
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while creating your channel";
                ModelState.AddModelError("", "An error occurred while creating your channel");
                return View("Index", model);
            }
        }
    }
}
