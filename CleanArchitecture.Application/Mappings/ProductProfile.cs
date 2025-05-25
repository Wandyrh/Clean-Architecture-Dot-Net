using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Mappings;

public class ProductProfile : Profile
{
    protected ProductProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
