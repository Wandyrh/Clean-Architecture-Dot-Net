using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators;

public class BaseEntityDtoValidator : AbstractValidator<BaseEntityDto>
{
    public BaseEntityDtoValidator()
    {
        RuleFor(_ => _.Id).NotNull().NotEmpty();
    }
}
