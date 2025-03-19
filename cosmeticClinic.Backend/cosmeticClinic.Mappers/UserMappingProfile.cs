using AutoMapper;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class UserMappingProfile: Profile
{
    public UserMappingProfile()
    {
        CreateMap<UserCreateDto,User>();

        CreateMap<UserDto,User>();

        CreateMap<User, UserDto>();
    }
}