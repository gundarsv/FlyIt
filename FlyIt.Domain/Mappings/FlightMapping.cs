using AutoMapper;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.AviationstackResponses;

using Entity = FlyIt.DataAccess.Entities;

namespace FlyIt.Domain.Mappings
{
    public class FlightMapping : Profile
    {
        public FlightMapping()
        {
            CreateMap<FlightsResponse, Entity.Flight>()
                .ForMember(f => f.FlightNo, options => options.MapFrom(fr => fr.Data[0].Flight.Iata))
                .ForMember(f => f.Date, options => options.MapFrom(fr => fr.Data[0].FlightDate))
                .ForMember(f => f.Status, options => options.MapFrom(fr => fr.Data[0].FlightStatus))
                .ForMember(f => f.DepartureGate, options => options.MapFrom(fr => fr.Data[0].Departure.Gate))
                .ForMember(f => f.DepartureDelay, options => options.MapFrom(fr => fr.Data[0].Departure.Delay))
                .ForMember(f => f.DepartureTerminal, options => options.MapFrom(fr => fr.Data[0].Departure.Terminal))
                .ForMember(f => f.DepartureAirportName, options => options.MapFrom(fr => fr.Data[0].Departure.Airport))
                .ForMember(f => f.DepartureScheduled, options => options.MapFrom(fr => fr.Data[0].Departure.Scheduled))
                .ForMember(f => f.DepartureEstimated, options => options.MapFrom(fr => fr.Data[0].Departure.Estimated))
                .ForMember(f => f.DepartureActual, options => options.MapFrom(fr => fr.Data[0].Departure.Actual))
                .ForMember(f => f.DepartureIata, options => options.MapFrom(fr => fr.Data[0].Departure.Iata))
                .ForMember(f => f.DestinationGate, options => options.MapFrom(fr => fr.Data[0].Arrival.Gate))
                .ForMember(f => f.DestinationDelay, options => options.MapFrom(fr => fr.Data[0].Arrival.Delay))
                .ForMember(f => f.DestinationTerminal, options => options.MapFrom(fr => fr.Data[0].Arrival.Terminal))
                .ForMember(f => f.DestinationAirportName, options => options.MapFrom(fr => fr.Data[0].Arrival.Airport))
                .ForMember(f => f.DestinationScheduled, options => options.MapFrom(fr => fr.Data[0].Arrival.Scheduled))
                .ForMember(f => f.DestinationEstimated, options => options.MapFrom(fr => fr.Data[0].Arrival.Estimated))
                .ForMember(f => f.DestinationActual, options => options.MapFrom(fr => fr.Data[0].Arrival.Actual))
                .ForMember(f => f.DestinationIata, options => options.MapFrom(fr => fr.Data[0].Arrival.Iata))
                .ForMember(f => f.DestinationIcao, options => options.MapFrom(fr => fr.Data[0].Arrival.Icao))
                .ForMember(f => f.DepartureIcao, options => options.MapFrom(fr => fr.Data[0].Departure.Icao));

            CreateMap<FlightsResponse, FlightDTO>()
                .ForMember(fdto => fdto.FlightNo, options => options.MapFrom(fr => fr.Data[0].Flight.Iata))
                .ForMember(fdto => fdto.Date, options => options.MapFrom(fr => fr.Data[0].FlightDate))
                .ForMember(fdto => fdto.Status, options => options.MapFrom(fr => fr.Data[0].FlightStatus))
                .ForPath(fdto => fdto.Departure.Gate, options => options.MapFrom(fr => fr.Data[0].Departure.Gate))
                .ForPath(fdto => fdto.Departure.Delay, options => options.MapFrom(fr => fr.Data[0].Departure.Delay))
                .ForPath(fdto => fdto.Departure.Terminal, options => options.MapFrom(fr => fr.Data[0].Departure.Terminal))
                .ForPath(fdto => fdto.Departure.AirportName, options => options.MapFrom(fr => fr.Data[0].Departure.Airport))
                .ForPath(fdto => fdto.Departure.Scheduled, options => options.MapFrom(fr => fr.Data[0].Departure.Scheduled))
                .ForPath(fdto => fdto.Departure.Estimated, options => options.MapFrom(fr => fr.Data[0].Departure.Estimated))
                .ForPath(fdto => fdto.Departure.Actual, options => options.MapFrom(fr => fr.Data[0].Departure.Actual))
                .ForPath(fdto => fdto.Departure.Iata, options => options.MapFrom(fr => fr.Data[0].Departure.Iata))
                .ForPath(fdto => fdto.Destination.Gate, options => options.MapFrom(fr => fr.Data[0].Arrival.Gate))
                .ForPath(fdto => fdto.Destination.Delay, options => options.MapFrom(fr => fr.Data[0].Arrival.Delay))
                .ForPath(fdto => fdto.Destination.Terminal, options => options.MapFrom(fr => fr.Data[0].Arrival.Terminal))
                .ForPath(fdto => fdto.Destination.AirportName, options => options.MapFrom(fr => fr.Data[0].Arrival.Airport))
                .ForPath(fdto => fdto.Destination.Scheduled, options => options.MapFrom(fr => fr.Data[0].Arrival.Scheduled))
                .ForPath(fdto => fdto.Destination.Estimated, options => options.MapFrom(fr => fr.Data[0].Arrival.Estimated))
                .ForPath(fdto => fdto.Destination.Actual, options => options.MapFrom(fr => fr.Data[0].Arrival.Actual))
                .ForPath(fdto => fdto.Destination.Iata, options => options.MapFrom(fr => fr.Data[0].Arrival.Iata))
                .ForPath(fdto => fdto.Destination.Icao, options => options.MapFrom(fr => fr.Data[0].Arrival.Icao))
                .ForPath(fdto => fdto.Departure.Icao, options => options.MapFrom(fr => fr.Data[0].Departure.Icao));

            CreateMap<FlightsResponse, FlightSearchDTO>()
                .ForMember(fsdto => fsdto.FlightNo, options => options.MapFrom(fr => fr.Data[0].Flight.Iata))
                .ForMember(fsdto => fsdto.Date, options => options.MapFrom(fdto => fdto.Data[0].FlightDate));

            CreateMap<Entity.Flight, FlightDTO>()
                .ForMember(fdto => fdto.Id, options => options.MapFrom(f => f.Id))
                .ForMember(fdto => fdto.FlightNo, options => options.MapFrom(f => f.FlightNo))
                .ForMember(fdto => fdto.Date, options => options.MapFrom(f => f.Date))
                .ForMember(fdto => fdto.Status, options => options.MapFrom(f => f.Status))
                .ForPath(fdto => fdto.Departure.Gate, options => options.MapFrom(f => f.DepartureGate))
                .ForPath(fdto => fdto.Departure.Delay, options => options.MapFrom(f => f.DepartureDelay))
                .ForPath(fdto => fdto.Departure.Terminal, options => options.MapFrom(f => f.DepartureTerminal))
                .ForPath(fdto => fdto.Departure.AirportName, options => options.MapFrom(f => f.DepartureAirportName))
                .ForPath(fdto => fdto.Departure.Scheduled, options => options.MapFrom(f => f.DepartureScheduled))
                .ForPath(fdto => fdto.Departure.Estimated, options => options.MapFrom(f => f.DepartureEstimated))
                .ForPath(fdto => fdto.Departure.Actual, options => options.MapFrom(f => f.DepartureActual))
                .ForPath(fdto => fdto.Departure.Iata, options => options.MapFrom(f => f.DepartureIata))
                .ForPath(fdto => fdto.Destination.Gate, options => options.MapFrom(f => f.DestinationGate))
                .ForPath(fdto => fdto.Destination.Delay, options => options.MapFrom(f => f.DestinationDelay))
                .ForPath(fdto => fdto.Destination.Terminal, options => options.MapFrom(f => f.DestinationTerminal))
                .ForPath(fdto => fdto.Destination.AirportName, options => options.MapFrom(f => f.DestinationAirportName))
                .ForPath(fdto => fdto.Destination.Scheduled, options => options.MapFrom(f => f.DestinationScheduled))
                .ForPath(fdto => fdto.Destination.Estimated, options => options.MapFrom(f => f.DestinationEstimated))
                .ForPath(fdto => fdto.Destination.Actual, options => options.MapFrom(f => f.DestinationActual))
                .ForPath(fdto => fdto.Destination.Iata, options => options.MapFrom(f => f.DestinationIata))
                .ForPath(fdto => fdto.Destination.Icao, options => options.MapFrom(f => f.DestinationIcao))
                .ForPath(fdto => fdto.Departure.Icao, options => options.MapFrom(f => f.DepartureIcao));

            CreateMap<Entity.UserFlight, FlightDTO>()
                .ForMember(fdto => fdto.Id, options => options.MapFrom(uf => uf.Flight.Id))
                .ForMember(fdto => fdto.FlightNo, options => options.MapFrom(uf => uf.Flight.FlightNo))
                .ForMember(fdto => fdto.Date, options => options.MapFrom(uf => uf.Flight.Date))
                .ForMember(fdto => fdto.Status, options => options.MapFrom(uf => uf.Flight.Status))
                .ForPath(fdto => fdto.Departure.Gate, options => options.MapFrom(uf => uf.Flight.DepartureGate))
                .ForPath(fdto => fdto.Departure.Delay, options => options.MapFrom(uf => uf.Flight.DepartureDelay))
                .ForPath(fdto => fdto.Departure.Terminal, options => options.MapFrom(uf => uf.Flight.DepartureTerminal))
                .ForPath(fdto => fdto.Departure.AirportName, options => options.MapFrom(uf => uf.Flight.DepartureAirportName))
                .ForPath(fdto => fdto.Departure.Scheduled, options => options.MapFrom(uf => uf.Flight.DepartureScheduled))
                .ForPath(fdto => fdto.Departure.Estimated, options => options.MapFrom(uf => uf.Flight.DepartureEstimated))
                .ForPath(fdto => fdto.Departure.Actual, options => options.MapFrom(uf => uf.Flight.DepartureActual))
                .ForPath(fdto => fdto.Departure.Iata, options => options.MapFrom(uf => uf.Flight.DepartureIata))
                .ForPath(fdto => fdto.Destination.Gate, options => options.MapFrom(uf => uf.Flight.DestinationGate))
                .ForPath(fdto => fdto.Destination.Delay, options => options.MapFrom(uf => uf.Flight.DestinationDelay))
                .ForPath(fdto => fdto.Destination.Terminal, options => options.MapFrom(uf => uf.Flight.DestinationTerminal))
                .ForPath(fdto => fdto.Destination.AirportName, options => options.MapFrom(uf => uf.Flight.DestinationAirportName))
                .ForPath(fdto => fdto.Destination.Scheduled, options => options.MapFrom(uf => uf.Flight.DestinationScheduled))
                .ForPath(fdto => fdto.Destination.Estimated, options => options.MapFrom(uf => uf.Flight.DestinationEstimated))
                .ForPath(fdto => fdto.Destination.Actual, options => options.MapFrom(uf => uf.Flight.DestinationActual))
                .ForPath(fdto => fdto.Destination.Iata, options => options.MapFrom(uf => uf.Flight.DestinationIata))
                .ForPath(fdto => fdto.Destination.Icao, options => options.MapFrom(uf => uf.Flight.DestinationIcao))
                .ForPath(fdto => fdto.Departure.Icao, options => options.MapFrom(uf => uf.Flight.DepartureIcao));
        }
    }
}