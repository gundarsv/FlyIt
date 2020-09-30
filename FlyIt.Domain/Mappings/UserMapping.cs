using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Models;

namespace FlyIt.Domain.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDTO>();

            CreateMap<UserDTO, User>()
                .ForMember(u => u.UserName, options => options.MapFrom(udto => udto.Email));
        }
    }
}
