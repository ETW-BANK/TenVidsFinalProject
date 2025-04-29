using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using TenVids.Application.Models;
using TenVids.Models.Pagination;
using TenVids.Services.IServices;
using TenVids.Utilities;

namespace TenVids.Application.Controllers
{
    public class HomeController : Controller
    {
      private readonly ILogger<HomeController> _logger; 
        private readonly IHomeService _homeService;
        private readonly IVideosService _videosService;
        private readonly ISideBarService _sidebarService;
       
        public HomeController(ILogger<HomeController> logger,IHomeService homeService, IVideosService videosService,ISideBarService sideBarService)
        {
            _logger = logger;
            _homeService = homeService;
            _videosService = videosService;
            _sidebarService = sideBarService;   
        
        }
        public async Task<IActionResult> Index(string page)
        {
            try
            {
                var result = await _homeService.GoToHomeAsync(page);
                return View(result);
            }
            //catch (UnauthorizedAccessException)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while processing your request. Please try again later. " + ex.Message;
                return View("Error");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region API CALLS
        //[Authorize(Roles = $"{SD.UserRole}")]
        [HttpGet]
      
        public async Task<IActionResult> GetVideosForHomeGrid(HomeParameters parameters)
        {
            try
            {
                var paginatedResult = await _videosService.GetVideosForHomeGridAsync(parameters);
                return Json(new ApiResponse(200,result:paginatedResult));
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while processing your request. Please try again later. " + ex.Message;
                return Json(new ApiResponse(500, message: "An error occurred while processing your request. Please try again later."));
            }
        }
        [Authorize(Roles = $"{SD.UserRole}")]
        [HttpGet]
        public async Task<IActionResult> GetSubscription()
        {
            var usrSubscribedChannels = await _sidebarService.GetSubscriptions();

            return Json(new ApiResponse(200,result:usrSubscribedChannels));
        }
        [Authorize(Roles = $"{SD.UserRole}")]
        [HttpGet]
        public async Task<IActionResult> GetHistories()
        {
            var usrHistories = await _sidebarService.GetHistory();

            return Json(new ApiResponse(200, result: usrHistories));
        }

        #endregion
    }
}
