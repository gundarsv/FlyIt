using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;
using System;

namespace FlyIt.Domain.Mappings
{
    public class AuthenticaiontMappingProfile : Profile
    {
        public AuthenticaiontMappingProfile()
        {
            CreateMap<UserToken, AuthenticationToken>()
                .ForMember(at => at.AccessToken, opt => opt.MapFrom(ut => ut.AccessToken))
                .ForMember(at => at.RefreshToken, opt => opt.MapFrom(ut => ut.RefreshToken))
                .ForMember(at => at.ExpiresAt, opt => opt.MapFrom(ut => new DateTimeOffset(ut.AccessTokenExpiration).ToUnixTimeMilliseconds()))
                .ForAllOtherMembers(x => x.Ignore());
        }
    }
}