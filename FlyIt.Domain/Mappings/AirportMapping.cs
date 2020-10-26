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
                .ForMember(adto => adto.MapName, options => options.MapFrom(a => a.MapName))
                .ForMember(adto => adto.MapUrl, options => options.MapFrom(a => a.MapUrl))
                .ForMember(adto => adto.Name, options => options.MapFrom(a => a.Name));

            CreateMap<UserAirport, AirportDTO>()
                 .ForMember(adto => adto.Id, options => options.MapFrom(ua => ua.Airport.Id))
                 .ForMember(adto => adto.Iata, options => options.MapFrom(ua => ua.Airport.Iata))
                 .ForMember(adto => adto.MapName, options => options.MapFrom(ua => ua.Airport.MapName))
                 .ForMember(adto => adto.MapUrl, options => options.MapFrom(ua => ua.Airport.MapUrl))
                 .ForMember(adto => adto.Name, options => options.MapFrom(ua => ua.Airport.Name));
        }
    }
}