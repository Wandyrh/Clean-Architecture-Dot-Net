using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators;

public class UpdateProductCategoryDtoValidator : AbstractValidator<UpdateProductCategoryDto>
{
    public UpdateProductCategoryDtoValidator()
    {
        RuleFor(_ => _.Id).NotNull().NotEmpty();
        RuleFor(_ => _.Name).NotNull().NotEmpty();
        RuleFor(_ => _.Description).NotNull().NotEmpty();
    }
}