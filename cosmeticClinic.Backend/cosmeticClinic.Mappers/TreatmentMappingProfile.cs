using AutoMapper;
using cosmeticClinic.DTOs.Product;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class TreatmentMappingProfile: Profile
{
    public TreatmentMappingProfile()
    {
        CreateMap<TreatmentCreateDto,Treatment>();

        CreateMap<TreatmentDto,Treatment>();

        CreateMap<Treatment, TreatmentDto>();
    }
}