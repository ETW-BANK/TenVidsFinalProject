using AutoMapper;
using TenVids.Models;
using TenVids.ViewModels;

namespace TenVids.Utilities.Mapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDisplayVM>()
                .ForMember(d => d.ChannelId, opt => opt.MapFrom(s => s.Channel == null ? 0 : s.Channel.Id))
                .ForMember(d => d.ChannelName, opt => opt.MapFrom(s => s.Channel == null ? "" : s.Channel.Name));

            CreateMap<ApplicationUser, UserAddEditVM>();
            CreateMap<Videos, VideoDisplayVm>()

                .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
                .ForMember(d => d.ChannelName, opt => opt.MapFrom(s => s.Channel.Name));
             

        }
    }
}