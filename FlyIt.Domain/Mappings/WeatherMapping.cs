using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;

namespace FlyIt.Domain.Mappings
{
    public class WeatherMapping : Profile
    {
        public WeatherMapping()
        {
            CreateMap<Weather, WeatherDTO>()
                .ForMember(wdto => wdto.Temperature, options => options.MapFrom(w => w.Temperature))
                .ForMember(wdto => wdto.WeatherStatus, options => options.MapFrom(w => w.WeatherStatus));
        }
    }
}