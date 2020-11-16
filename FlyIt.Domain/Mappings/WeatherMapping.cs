using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.MetarResponse;

namespace FlyIt.Domain.Mappings
{
    public class WeatherMapping : Profile
    {
        public WeatherMapping()
        {
            CreateMap<MetarResponse, WeatherDTO>()
                .ForMember(wdto => wdto.Temperature, options => options.MapFrom(mr => mr.Data[0].Temperature.Celsius))
                .ForMember(wdto => wdto.RealFeel, options => options.MapFrom(mr => mr.Data[0].WindChill.Celsius))
                .ForMember(wdto => wdto.Humidity, options => options.MapFrom(mr => mr.Data[0].Humidity.Percent))
                .ForMember(wdto => wdto.WindSpeed, options => options.MapFrom(mr => mr.Data[0].Wind.SpeedKph));
        }
    }
}