using AutoMapper;
using cosmeticClinic.DTOs.Product;
using cosmeticClinic.Entities;

namespace cosmeticClinic.Mappers;

public class ProductMappingProfile: Profile
{
    public ProductMappingProfile()
    {
        CreateMap<ProductCreateDto,Product>();

        CreateMap<ProductDto,Product>();

        CreateMap<Product, ProductDto>();
    }
}