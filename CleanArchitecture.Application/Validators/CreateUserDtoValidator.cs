using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators;

public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserDtoValidator()
    {
        RuleFor(_ => _.FirstName).NotNull().NotEmpty();
        RuleFor(_ => _.LastName).NotNull().NotEmpty();        
        RuleFor(_ => _.Email).NotNull().NotEmpty().EmailAddress();
        RuleFor(_ => _.Password).NotNull().NotEmpty().MinimumLength(8);
    }
}