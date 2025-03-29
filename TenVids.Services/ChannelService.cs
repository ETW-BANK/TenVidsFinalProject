
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;
using TenVids.ViewModels;
using TenVids.Services.Extensions;
using Microsoft.AspNetCore.Http;

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
    }
}
