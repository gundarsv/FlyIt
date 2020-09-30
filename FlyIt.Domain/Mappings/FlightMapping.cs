using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.AviationstackResponses;

using Entity = FlyIt.DataAccess.Entities;

namespace FlyIt.Domain.Mappings
{
    public class FlightMapping : Profile
    {
        public FlightMapping()
        {
            CreateMap<FlightsResponse, NoIdFlightDTO>()
                .ForMember(fdto => fdto.FlightNo, options => options.MapFrom(fr => fr.Data[0].Flight.Iata))
                .ForMember(fdto => fdto.Date, options => options.MapFrom(fr => fr.Data[0].FlightDate));

            CreateMap<FlightsResponse, Entity.Flight>()
                .ForMember(f => f.FlightNo, options => options.MapFrom(fr => fr.Data[0].Flight.Iata))
                .ForMember(f => f.Date, options => options.MapFrom(fr => fr.Data[0].FlightDate));

            CreateMap<Entity.UserFlight, FlightDTO>()
                .ForMember(f => f.Id, options => options.MapFrom(uf => uf.Flight.Id))
                .ForMember(f => f.FlightNo, options => options.MapFrom(uf => uf.Flight.FlightNo))
                .ForMember(f => f.Date, options => options.MapFrom(uf => uf.Flight.Date));

            CreateMap<FlightDTO, Entity.Flight>()
                .ForMember(f => f.Id, options => options.MapFrom(fdto => fdto.Id))
                .ForMember(f => f.FlightNo, options => options.MapFrom(fdto => fdto.FlightNo))
                .ForMember(f => f.Date, options => options.MapFrom(fdto => fdto.Date));
        }
    }
}
