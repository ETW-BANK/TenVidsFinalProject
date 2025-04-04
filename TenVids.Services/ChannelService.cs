
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;
using TenVids.ViewModels;
using Microsoft.AspNetCore.Http;
using TenVids.Services.Extensions;
using TenVids.Models;
using TenVids.Utilities;

namespace TenVids.Services
{
    public class ChannelService:IChannelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ChannelService(IUnitOfWork unitOfWork, IHttpContextAccessor? httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ChannelAddEditVM> GetUserChannelAsync()
        {
            var model=new ChannelAddEditVM();   
            var channel = await _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(x => x.AppUserId == _httpContextAccessor.HttpContext.User.GetUserId());
            if (channel != null)
            {
                model.Name = channel.Name;
                model.Description = channel.Description;
                return model;
            }
            return null;

        }

        public async Task<ChannelCreationResult> CreateChannelAsync(ChannelAddEditVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            if (string.IsNullOrEmpty(userId))
                return ChannelCreationResult.Failure("User not authenticated");

            if (await _unitOfWork.ChannelRepository.UserHasChannelAsync(userId))
                return ChannelCreationResult.Failure("You can only create one channel per account");

            var normalizedName = model.Name?.Trim().ToLower();
            var existingChannel = await _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(c =>
                c.Name.ToLower() == normalizedName);

            if (existingChannel != null)
            {
                if (existingChannel.AppUserId == userId)
                    return ChannelCreationResult.Success(); 

                return ChannelCreationResult.Failure($"Channel name '{model.Name}' is already taken");
            }

            var channel = new Channel
            {
                Name = model.Name.Trim(),
                Description = model.Description?.Trim(),
                AppUserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.ChannelRepository.Add(channel);
            await _unitOfWork.CompleteAsync();

            return ChannelCreationResult.Success();
        }
        public async Task<bool> UserHasChannelAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
            return !string.IsNullOrEmpty(userId) &&
                   await _unitOfWork.ChannelRepository.UserHasChannelAsync(userId);
        }
    }
}
