using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;
using FlyIt.Domain.Models.Enums;

namespace FlyIt.Domain.Mappings
{
    public class ChatroomMessageMapping : Profile
    {
        public ChatroomMessageMapping()
        {
            CreateMap<ChatroomMessage, ChatroomMessageDTO>()
                .ForMember(crmdto => crmdto.UserName, options => options.MapFrom(crm => crm.User.UserName))
                .ForMember(crmdto => crmdto.Message, options => options.MapFrom(crm => crm.Message))
                .ForMember(crmdto => crmdto.DateTime, options => options.MapFrom(crm => crm.DateTime))
                .ForMember(crmdto => crmdto.MessageType, options => options.MapFrom(crm => MessageType.Message));
        }
    }
}