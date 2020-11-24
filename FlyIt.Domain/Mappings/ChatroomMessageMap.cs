using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;

namespace FlyIt.Domain.Mappings
{
    public class ChatroomMessageMap : Profile
    {
        public ChatroomMessageMap()
        {
            CreateMap<ChatroomMessage, ChatroomMessageDTO>()
                .ForMember(crmdto => crmdto.UserName, options => options.MapFrom(crm => crm.User.UserName))
                .ForMember(crmdto => crmdto.Message, options => options.MapFrom(crm => crm.Message))
                .ForMember(crmdto => crmdto.DateTime, options => options.MapFrom(crm => crm.DateTime));
        }
    }
}