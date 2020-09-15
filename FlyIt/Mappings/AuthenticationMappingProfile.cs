using AutoMapper;
using FlyIt.DataContext.Entities.Identity;
using FlyIt.Services.Models;
using System;

namespace FlyIt.Api.Mappings
{
    public class AuthenticaiontMappingProfile : Profile
    {
        public AuthenticaiontMappingProfile()
        {
            CreateMap<UserToken, AuthenticationToken>()
                .ForMember(at => at.AccessToken, opt => opt.MapFrom(ut => ut.Value))
                .ForMember(at => at.RefreshToken, opt => opt.MapFrom(ut => ut.RefreshToken))
                .ForMember(at => at.ExpiresAt, opt => opt.MapFrom(ut => ut.AccessTokenExpiration));
        }
    }
}
