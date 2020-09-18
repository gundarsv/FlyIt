using AutoMapper;
using FlyIt.Api.Models;

using Entity = FlyIt.DataContext.Entities.Identity;

namespace FlyIt.Api.Mappings
{
    public class SignUpMappingProfile : Profile
    {
        public SignUpMappingProfile()
        {
            CreateMap<SignUp, Entity.User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(ur => ur.Email));
        }
    }
}
