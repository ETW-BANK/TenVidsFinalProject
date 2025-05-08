using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;

namespace TenVids.Application.Controllers
{
    [Authorize(Roles = SD.ModeratorRole)]
    public class ModeratorController : Controller
    {
        private readonly IVideosService _videosService;

        public ModeratorController(IVideosService videosService)
        {
            _videosService = videosService;
        }
        public async Task<IActionResult> AllVideos()
        {
            var result = await _videosService.AllVideos();

            if (result.IsSuccess)
            {
                return View(result.Data); 
            }

       
            ViewBag.ErrorMessage = result.Message;
            return View(new List<VideoDisplayVm>()); 
        }

    }
}
