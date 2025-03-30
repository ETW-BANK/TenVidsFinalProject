
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;
using TenVids.ViewModels;
using Microsoft.AspNetCore.Http;
using TenVids.Services.Extensions;
using TenVids.Models;

namespace TenVids.Services
{
    public class ChannelService:IChannelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChannelService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChannelAddEditVM> GetUserChannelAsync()
        {
            var userId=_httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return new ChannelAddEditVM();
            var channel = await _unitOfWork.ChannelRepository.GetByUserIdAsync(userId, "Videos,Subscribers");

            return channel == null ? new ChannelAddEditVM() : new ChannelAddEditVM
            {
                Name = channel.Name,
                Description = channel.Description,
             
            };
        }
        public async Task CreateChannelAsync(ChannelAddEditVM model)
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            if (await _unitOfWork.ChannelRepository.UserHasChannelAsync(userId))
                throw new InvalidOperationException("User already has a channel");

            var channel = new Channel
            {
                Name = model.Name,
                Description = model.Description,
                AppUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ChannelRepository.CreateAsync(channel);
        }

        public async Task<bool> UserHasChannelAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            return !string.IsNullOrEmpty(userId) &&
                   await _unitOfWork.ChannelRepository.UserHasChannelAsync(userId);
        }
    }
}
