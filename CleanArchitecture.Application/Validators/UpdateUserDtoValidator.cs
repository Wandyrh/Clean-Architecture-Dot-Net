using CleanArchitecture.Application.DTOs;
using FluentValidation;

namespace CleanArchitecture.Application.Validators;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(_ => _.FirstName).NotNull().NotEmpty();
        RuleFor(_ => _.LastName).NotNull().NotEmpty();
        RuleFor(_ => _.Email).NotNull().NotEmpty().EmailAddress();        
    }
}
