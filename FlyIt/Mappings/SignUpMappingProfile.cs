using AutoMapper;
using FlyIt.Api.Models;
using FlyIt.DataContext.Entities.Identity;

namespace FlyIt.Api.Mappings
{
    public class SignUpMappingProfile : Profile
    {
        public SignUpMappingProfile()
        {
            CreateMap<SignUp, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
        }
    }
}
