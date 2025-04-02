using AutoMapper;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class MessageMappingProfile: Profile
{
    public MessageMappingProfile()
    {
        CreateMap<MessageDto,Message>();

        CreateMap<Message, MessageDto>();
    }
}