using AutoMapper;
using cosmeticClinic.DTOs.Appointment;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class AppointmentMappingProfile: Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<AppointmentCreateDto,Appointment>();

        CreateMap<AppointmentDto,Appointment>();

        CreateMap<Appointment, AppointmentDto>();
        
        CreateMap<Appointment, AppointmentDetailsDto>()
            .ForMember(dest => dest.PatientName, opt => opt.Ignore())
            .ForMember(dest => dest.DoctorName, opt => opt.Ignore())
            .ForMember(dest => dest.TreatmentName, opt => opt.Ignore());
    }
}