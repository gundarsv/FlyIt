using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;

namespace FlyIt.Domain.Mappings
{
    public class AirportMapping : Profile
    {
        public AirportMapping()
        {
            CreateMap<Airport, AirportDTO>()
                .ForMember(adto => adto.Id, options => options.MapFrom(a => a.Id))
                .ForMember(adto => adto.Iata, options => options.MapFrom(a => a.Iata))
                .ForMember(adto => adto.Name, options => options.MapFrom(a => a.Name));

            CreateMap<UserAirport, AirportDTO>()
                 .ForMember(adto => adto.Id, options => options.MapFrom(a => a.Airport.Id))
                 .ForMember(adto => adto.Iata, options => options.MapFrom(a => a.Airport.Iata))
                 .ForMember(adto => adto.Name, options => options.MapFrom(a => a.Airport.Name));
        }
    }
}