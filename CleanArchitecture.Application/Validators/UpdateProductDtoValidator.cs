using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(_ => _.Id).NotNull().NotEmpty();
        RuleFor(_ => _.CategoryId).NotNull().NotEmpty();
        RuleFor(_ => _.Name).NotNull().NotEmpty();
        RuleFor(_ => _.Description).NotNull().NotEmpty();
    }
}

