
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
            var ChannelExists= await _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(x=>x.Name==model.Name);

            if (ChannelExists != null)
            {
                return ChannelCreationResult.Failure("A channel with this name already exists.");
            }
            var newchannel = new Channel
            {
                Name = model.Name,
                Description = model.Description,
                AppUserId = _httpContextAccessor.HttpContext.User.GetUserId()
            };
            _unitOfWork.ChannelRepository.Add(newchannel);
           await _unitOfWork.CompleteAsync();

            return ChannelCreationResult.Success();

        }
       
    }
}
