using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(_ => _.CategoryId).NotNull().NotEmpty();
        RuleFor(_ => _.Name).NotNull().NotEmpty();
        RuleFor(_ => _.Description).NotNull().NotEmpty();
    }
}
