using AutoMapper;
using FlyIt.Api.Models;
using FlyIt.DataContext.Entities.Identity;

namespace FlyIt.Api.Mappings
{
    public class SignUnMappingProfile : Profile
    {
        public SignUnMappingProfile()
        {
            CreateMap<SignUp, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
        }
    }
}
