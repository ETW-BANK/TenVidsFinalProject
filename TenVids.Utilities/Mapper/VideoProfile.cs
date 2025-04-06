using AutoMapper;
using TenVids.Models;
using TenVids.ViewModels;

namespace TenVids.Utilities.Mapper
{
    public class VideoProfile:Profile
    {
        public VideoProfile()
        {
            CreateMap<Videos, VideoVM>()
                .ForMember(dest => dest.CategoryList, opt => opt.Ignore())
                .ForMember(dest => dest.ImageContentTypes, opt => opt.Ignore())
                .ForMember(dest => dest.VideoContentTypes, opt => opt.Ignore());
        }
    }
}
