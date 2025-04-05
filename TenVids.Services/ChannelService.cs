
using TenVids.Repository.IRepository;
using TenVids.Services.IServices;
using TenVids.ViewModels;
using Microsoft.AspNetCore.Http;
using TenVids.Services.Extensions;
using TenVids.Models;
using TenVids.Utilities;
using Microsoft.EntityFrameworkCore;


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

        public async Task<ErrorModel<Channel>> CreateChannelAsync(ChannelAddEditVM model)
        {
            var ChannelExists= await _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(x=>x.Name==model.Name);

            if (ChannelExists != null)
            {
                return ErrorModel<Channel>.Failure("Channel with this name already exists.",409);
            }
            var newchannel = new Channel
            {
                Name = model.Name,
                Description = model.Description,
                AppUserId = _httpContextAccessor.HttpContext.User.GetUserId()
            };
            _unitOfWork.ChannelRepository.Add(newchannel);
           await _unitOfWork.CompleteAsync();

            return ErrorModel<Channel>.Success(newchannel,"Channel Created Succesfully");

        }
        public async Task<ErrorModel<Channel>> UpdateChannelAsync(ChannelAddEditVM model)
        {
        
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                return ErrorModel<Channel>.Failure("Channel name cannot be empty.", 400);
            }

            var currentUserId = _httpContextAccessor.HttpContext.User.GetUserId();

            var existingChannel = await _unitOfWork.ChannelRepository
                .GetFirstOrDefaultAsync(x => x.AppUserId == currentUserId, tracked: true);

            if (existingChannel == null)
            {
                return ErrorModel<Channel>.Failure("You don't have a channel to update.", 404);
            }
            var normalizedName = model.Name.Trim().ToLower();
            if (!existingChannel.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase))
            {
                var duplicateExists = await _unitOfWork.ChannelRepository
                    .GetFirstOrDefaultAsync(x =>
                        x.Name.ToLower() == normalizedName &&
                        x.Id != existingChannel.Id);

                if (duplicateExists != null)
                {
                    return ErrorModel<Channel>.Failure("Channel name already exists.", 409);
                }
            }

            existingChannel.Name = model.Name.Trim();
            existingChannel.Description = model.Description?.Trim();

            try
            {
                await _unitOfWork.CompleteAsync();
                return ErrorModel<Channel>.Success(existingChannel, "Channel updated successfully");
            }
            catch (DbUpdateException ex)
            {
                
                return ErrorModel<Channel>.Failure("Failed to update channel.", 500);
            }
        }

        public async Task DeleteChannelAsync(Channel model)
        {
            model = _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(c => c.Id == model.Id).Result;

            if (model== null)
            {
                throw new Exception("Channel not found.");
            }

            _unitOfWork.ChannelRepository.Remove(model);
            await _unitOfWork.CompleteAsync();
        }

        public Task<Channel> GetChannelByIdAsync(int id)
        {
            var channel = _unitOfWork.ChannelRepository.GetFirstOrDefaultAsync(c => c.Id == id);
            if (channel == null)
            {
                throw new Exception("Category not found.");
            }


            return channel;
        }
    }
}
