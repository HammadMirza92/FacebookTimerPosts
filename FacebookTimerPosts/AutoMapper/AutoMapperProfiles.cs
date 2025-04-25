using AutoMapper;
using FacebookTimerPosts.DTOs;
using FacebookTimerPosts.Models;

namespace FacebookTimerPosts.AutoMapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.SubscriptionType, opt =>
                    opt.MapFrom(src => src.SubscriptionType.ToString()))
                .ForMember(dest => dest.LinkedPagesCount, opt =>
                    opt.MapFrom(src => src.LinkedPages.Count));

            CreateMap<FacebookPage, FacebookPageDto>();

            CreateMap<Template, TemplateDto>()
                .ForMember(dest => dest.Category, opt =>
                    opt.MapFrom(src => src.Category.ToString()))
                .ForMember(dest => dest.MinimumSubscription, opt =>
                    opt.MapFrom(src => src.MinimumSubscription.ToString()));

            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Status, opt =>
                    opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreatePostDto, Post>();
            CreateMap<UpdatePostDto, Post>();

            CreateMap<Post, CountdownViewDto>();
        }
    }
}
