using AutoMapper;
using cosmeticClinic.DTOs.Doctor;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class DoctorMappingProfile : Profile
{
    public DoctorMappingProfile()
    {
        CreateMap<DoctorCreateDto, Doctor>();
        
        CreateMap<DoctorDto, Doctor>();
        
        CreateMap<Doctor, DoctorDto>();
        
        CreateMap<WorkingHoursDto, WorkingHours>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));
        
        CreateMap<WorkingHours, WorkingHoursDto>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime));
    }
}