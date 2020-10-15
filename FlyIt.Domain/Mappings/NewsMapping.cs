using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyIt.Domain.Mappings
{
    public class NewsMapping : Profile
    {
        public NewsMapping()
        {
            CreateMap<News, NewsDTO>()
                .ForMember(ndto => ndto.Id, options => options.MapFrom(n => n.Id))
                .ForMember(ndto => ndto.Imageurl, options => options.MapFrom(n => n.Imageurl))
                .ForMember(ndto => ndto.Title, options => options.MapFrom(n => n.Title))
                .ForMember(ndto => ndto.Body, options => options.MapFrom(n => n.Body))
                .ForMember(ndto => ndto.Airport, options => options.MapFrom(n => n.Airport));
        }
    }
}