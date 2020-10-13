using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Models;
using System.Linq;

namespace FlyIt.Domain.Mappings
{
    public class UserMapping : Profile
    {
        public UserMapping()
        {
            CreateMap<User, UserDTO>()
                .ForMember(udto => udto.Airports, options => options.MapFrom(u => u.UserAirports.Select(u => u.Airport)));

            CreateMap<UserDTO, User>()
                .ForMember(u => u.UserName, options => options.MapFrom(udto => udto.Email));
        }
    }
}