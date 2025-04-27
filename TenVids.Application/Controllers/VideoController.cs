using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenVids.Application.Controllers.Requests;
using System;
using System.Threading.Tasks;
using TenVids.Models.Pagination;
using TenVids.Services.IServices;
using TenVids.Utilities;
using TenVids.ViewModels;
using System.Linq;
using TenVids.Services.Extensions;


namespace TenVids.Application.Controllers
{
    [Authorize(Roles = $"{SD.UserRole}")]
    public class VideoController : Controller
    {
        private readonly IVideosService _videosService;
        private readonly IChannelService _channelService;
        private readonly ICommentService _commentService;
        public VideoController(IVideosService videosService,IChannelService channelService, ICommentService commentService)
        {
            _videosService = videosService;
            _channelService = channelService;
            _commentService = commentService;
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
        public async Task<IActionResult> WatchVideos(int id)
        {
            var result=await _videosService.GetVideoToWatchAsync(id);
            if (result == null)
            {
                TempData["error"] = "Video not found";
                return RedirectToAction("Index", "Home");
            }

            return View(result);
        }
        public async Task<IActionResult> GetVideoFile(int? videoId)
        {
            try
            {
                if (!videoId.HasValue || videoId <= 0)
                {
                    TempData["error"] = "Invalid video request";
                    return RedirectToAction("Index", "Home");
                }

                var videoFile = await _videosService.GetVideoFileAsync(videoId.Value);

                if (videoFile == null)
                {
                    TempData["error"] = "Video not found";
                    return RedirectToAction("Index", "Home");
                }

                return File(videoFile.Contents, videoFile.ContentType);
            }
            catch (Exception ex)
            { 
                TempData["error"] = "Error loading video";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> DownloadVideo(int videoId)
        {
            var result = await _videosService.DownloadVideoFileAsync(videoId);
            if (result==null)
            {
                TempData["error"] = result.Message;
                return RedirectToAction("Index", "Home");
            }

            return File(result.Data.Contents,result.Data.ContentType,result.Data.FileName);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentsVM comments)
        {
            
            var result = await _commentService.CreateCommentsAsync(comments);

            if (!result.IsSuccess)
            {
                TempData["error"] = result.Message;
            }
            else
            {
                TempData["success"] = result.Message;
            }

            return RedirectToAction("WatchVideos", new { id = comments.PostComment.VideoId });
        }





        #region API CALLS
        [HttpGet]
        public async Task<IActionResult>GetVideosForChannelGrid(BaseParams parameters)
        {
            var result=await _videosService.GetVideosForChannelAsync(parameters);

            return Json(new ApiResponse(200, result: result));
        }
        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVideo([FromBody] RequestParams request)
        {
            try
            {
                if (request?.Id <= 0)
                {
                    return Json(new ApiResponse(400, "Invalid video ID"));
                }
                var result = await _videosService.DeleteVideoAsync(request.Id);
                return Json(new ApiResponse(result.IsSuccess ? 200 : result.StatusCode, result.Message));
            }
            catch (Exception ex)
            {
                return Json(new ApiResponse(500, "An error occurred while deleting the video"));
            }
        }
        [HttpPut]
        public async Task<IActionResult> SubscribeChannel(int channelId)
        {
            var result = await _channelService.Subscribe(channelId);

            if (!result.IsSuccess)
            {
                return Json(new
                {
                    title = "Error",
                    message = result.Message,
                    isSuccess = false
                });
            }
            else if (result.Data.Subscribers == null || !result.Data.Subscribers.Any())
            {
                return Json(new
                {
                    title = "Unsubscribed",
                    message = "You've unsubscribed from this channel",
                    isSuccess = true
                });
            }
            else if (result.Data.Subscribers.Any(s => s.AppUserId == User.GetUserId() && s.ChannelId == channelId))
            {
                return Json(new
                {
                    title = "Subscribed",
                    message = "You've subscribed to this channel",
                    isSuccess = true
                });
            }

            return Json(new
            {
                title = "Error",
                message = "Channel not found",
                isSuccess = false
            });
        }

        [HttpPut]
        public async Task<IActionResult> LikeVideo(int videoId, string action,bool like)
        {
            var result = await _videosService.LikeVideo(videoId, action,like);

            if (result.StatusCode == 200)
            {
                return Ok(new
                {
                    success = true,
                    command = result.Data, // The clientCommand ("addLike", "removeLike", etc.)
                    message = result.Message ?? "Action completed successfully"
                });
            }
            else
            {
                return StatusCode(result.StatusCode, new
                {
                    success = false,
                    message = result.Message,
                    errors = result.Errors
                });
            }
        }

        #endregion
    }
}
