using AutoMapper;
using FlyIt.DataAccess.Entities.Identity;
using FlyIt.Domain.Models;

namespace FlyIt.Domain.Mappings
{
    public class RoleMapping : Profile
    {
        public RoleMapping()
        {
            CreateMap<Role, RoleDTO>()
                .ForMember(rdto => rdto.Id, options => options.MapFrom(r => r.Id))
                .ForMember(rdto => rdto.Name, options => options.MapFrom(r => r.Name));
        }
    }
}
