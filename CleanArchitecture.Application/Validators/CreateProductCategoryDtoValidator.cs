using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators;

public class CreateProductCategoryDtoValidator : AbstractValidator<CreateProductCategoryDto>
{
    public CreateProductCategoryDtoValidator()
    {
        RuleFor(_ => _.Name).NotNull().NotEmpty();
        RuleFor(_ => _.Description).NotNull().NotEmpty();
    }
}