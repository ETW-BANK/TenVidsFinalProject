using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TenVids.Models.Pagination;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles = $"{SD.UserRole}")]
    public class VideoController : Controller
    {

        private readonly IVideosService _videosService;

        public VideoController(IVideosService videosService)
        {
            _videosService = videosService;
        }
        [HttpGet]
        public async Task<IActionResult> Upsert(int id)
        {
            if (!await _videosService.UserHasChannelAsync())
            {
                TempData["error"] = "You need to create a channel first";
                return RedirectToAction("CreateChannel", "Channel");
            }

            try
            {
                var videoVM = await _videosService.GetVideoByIdAsync(id);
                if (videoVM == null)
                {
                    TempData["error"] = "Video not found";
                    return RedirectToAction("Index", "Channel");
                }
                return View(videoVM);
            }
            catch (InvalidOperationException ex)
            {
                TempData["error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(VideoVM videoVM)
        {
            if (!ModelState.IsValid)
            {
                videoVM = await _videosService.GetVideoByIdAsync(videoVM.Id);
                return View(videoVM);
            }

            var result = await _videosService.CreateEditVideoAsync(videoVM);

            if (!result.IsSuccess)
            {
                TempData["error"] = result.Message;
                videoVM = await _videosService.GetVideoByIdAsync(videoVM.Id);
                return View(videoVM);
            }

            TempData["success"] = result.Message;
            return RedirectToAction("Index", "Channel");
        }
        [HttpGet]
        public async Task<IActionResult>GetVideosForChannelGrid(BaseParams parameters)
        {
            var result=await _videosService.GetVideosForChannelAsync(parameters);

            return Json(new ApiResponse(200, result: result));
        }
    }
}
