using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    //WG --> Mapping
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            // Source -> Target
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>();
            CreateMap<PlatformReadDto, PlatformPublishedDto>();
            CreateMap<Platform, GrpcPlatformModel>()
                .ForMember(dest => 
                    dest.PlatformId, opt => 
                        opt.MapFrom(src => src.Id))
                .ForMember(dest => 
                    dest.Name, opt => 
                        opt.MapFrom(src => src.Name))
                .ForMember(dest => 
                    dest.Published, opt => 
                        opt.MapFrom(src => src.Publisher));
        }
    }
}