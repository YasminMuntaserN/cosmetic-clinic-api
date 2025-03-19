using AutoMapper;
using cosmeticClinic.DTOs;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class PatientMappingProfile: Profile
{
    public PatientMappingProfile()
    {
        CreateMap<PatientCreateDto,Patient>();

        CreateMap<PatientDto,Patient>();

        CreateMap<Patient, PatientDto>();
    }
}