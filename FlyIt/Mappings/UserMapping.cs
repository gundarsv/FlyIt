using AutoMapper;
using FlyIt.Api.Models;
using Entity = FlyIt.DataContext.Entities.Identity;

namespace FlyIt.Api.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<Entity.User, User>()
                .ForMember(u => u.Id, opt => opt.MapFrom(iu => iu.Id))
                .ForMember(u => u.UserName, opt => opt.MapFrom(iu => iu.UserName));
        }
    }
}
