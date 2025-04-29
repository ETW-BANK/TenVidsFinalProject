using Microsoft.EntityFrameworkCore;
using TenVids.Repository.IRepository;
using TenVids.Services.HelperMethods;
using TenVids.Services.IServices;
using TenVids.Utilities;

namespace TenVids.Services
{
    public class VideoViewService : IVideoViewService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHelper _helper;

        public VideoViewService(IUnitOfWork unitOfWork, IHelper helper)
        {
            _unitOfWork = unitOfWork;
            _helper = helper;
        }

        public async Task HandleVideoViewAsync(string userId, int videoId, string ipAddress)
        {
            var normalizedIp = SD.NormalizeIp(ipAddress);
            var now = DateTime.UtcNow;

            var existingView = await _unitOfWork.VideoViewRepository.GetQueryable()
                .Where(v => v.VideoId == videoId &&
                            v.AppUserId == userId &&
                            (v.IpAddress == normalizedIp || SD.LocalIpAddresses.Contains(v.IpAddress)))
                .OrderByDescending(v => v.LastVisit)
                .FirstOrDefaultAsync();

            if (existingView == null)
            {
               
                await _helper.AddVideoViewAsync(userId, videoId, normalizedIp);
            }
            else
            {
                if (existingView.LastVisit.Date < now.Date)
                {
                    
                    await _helper.AddVideoViewAsync(userId, videoId, normalizedIp);
                }
                else if (now > existingView.LastVisit.AddHours(1))
                {
                   
                    existingView.LastVisit = now;
                    existingView.NumberOfVisits++;
                    _unitOfWork.VideoViewRepository.Update(existingView);
                    await _unitOfWork.CompleteAsync();
                }
               
            }
        }
    }
}
