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
                .ForMember(adto => adto.Name, options => options.MapFrom(a => a.Name))
                .ForMember(adto => adto.RentingCompanyUrl, options => options.MapFrom(a => a.RentingCompanyUrl))
                .ForMember(adto => adto.RentingCompanyName, options => options.MapFrom(a => a.RentingCompanyName))
                .ForMember(adto => adto.RentingCompanyPhoneNo, options => options.MapFrom(a => a.RentingCompanyPhoneNo))
                .ForMember(adto => adto.TaxiPhoneNo, options => options.MapFrom(a => a.TaxiPhoneNo))
                .ForMember(adto => adto.EmergencyPhoneNo, options => options.MapFrom(a => a.EmergencyPhoneNo));

            CreateMap<UserAirport, AirportDTO>()
                 .ForMember(adto => adto.Id, options => options.MapFrom(ua => ua.Airport.Id))
                 .ForMember(adto => adto.Iata, options => options.MapFrom(ua => ua.Airport.Iata))
                 .ForMember(adto => adto.MapName, options => options.MapFrom(ua => ua.Airport.MapName))
                 .ForMember(adto => adto.MapUrl, options => options.MapFrom(ua => ua.Airport.MapUrl))
                 .ForMember(adto => adto.Name, options => options.MapFrom(ua => ua.Airport.Name))
                 .ForMember(adto => adto.RentingCompanyUrl, options => options.MapFrom(ua => ua.Airport.RentingCompanyUrl))
                 .ForMember(adto => adto.RentingCompanyName, options => options.MapFrom(ua => ua.Airport.RentingCompanyName))
                 .ForMember(adto => adto.RentingCompanyPhoneNo, options => options.MapFrom(ua => ua.Airport.RentingCompanyPhoneNo))
                 .ForMember(adto => adto.TaxiPhoneNo, options => options.MapFrom(ua => ua.Airport.TaxiPhoneNo))
                 .ForMember(adto => adto.EmergencyPhoneNo, options => options.MapFrom(ua => ua.Airport.EmergencyPhoneNo));
        }
    }
}