using AutoMapper;
using FlyIt.DataAccess.Entities;
using FlyIt.Domain.Models;

namespace FlyIt.Domain.Mappings
{
    public class ChatroomMapping : Profile
    {
        public ChatroomMapping()
        {
            CreateMap<Chatroom, ChatroomDTO>()
                .ForMember(crdto => crdto.FlightNo, options => options.MapFrom(cr => cr.Flight.FlightNo))
                .ForMember(crdto => crdto.Id, options => options.MapFrom(cr => cr.Id))
                .ForMember(crdto => crdto.Date, options => options.MapFrom(cr => cr.Flight.Date))
                .ForMember(crdto => crdto.ChatroomMessages, options => options.MapFrom(cr => cr.ChatroomMessages));

            CreateMap<UserChatroom, ChatroomDTO>()
                .ForMember(crdto => crdto.FlightNo, options => options.MapFrom(ucr => ucr.Chatroom.Flight.FlightNo))
                .ForMember(crdto => crdto.Id, options => options.MapFrom(ucr => ucr.Chatroom.Id))
                .ForMember(crdto => crdto.Date, options => options.MapFrom(ucr => ucr.Chatroom.Flight.Date))
                .ForMember(crdto => crdto.ChatroomMessages, options => options.MapFrom(ucr => ucr.Chatroom.ChatroomMessages));
        }
    }
}