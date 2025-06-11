using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Mappings;

public class ProductCategoryProfile : Profile
{
    public ProductCategoryProfile()
    {
        CreateMap<ProductCategory, ProductCategoryDto>().ReverseMap();
        CreateMap<ProductCategory, CreateProductCategoryDto>().ReverseMap();
        CreateMap<ProductCategory, UpdateProductCategoryDto>().ReverseMap();
    }
}