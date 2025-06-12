using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ReverseMap();
        CreateMap<Product, CreateProductDto>().ReverseMap();
        CreateMap<Product, UpdateProductDto>().ReverseMap();
    }
}
