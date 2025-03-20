using AutoMapper;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class DoctorMappingProfile: Profile
{
    public DoctorMappingProfile()
    {
        CreateMap<DoctorCreateDto,Doctor>();

        CreateMap<DoctorDto,Doctor>();

        CreateMap<Doctor, DoctorDto>();
    }
}