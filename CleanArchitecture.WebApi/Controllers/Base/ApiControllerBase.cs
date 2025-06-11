using CleanArchitecture.Application.DTOs;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers.Base
{
    public abstract class ApiControllerBase<T> : ControllerBase where T : class
    {
        protected readonly ILogger<T> _logger;
        protected readonly Dictionary<Type, IValidator> _validators;

        protected ApiControllerBase(ILogger<T> logger, Dictionary<Type, IValidator> validators)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _validators = validators ?? throw new ArgumentNullException(nameof(_validators));
        }

        protected async Task ValidateRequest(object request)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request), "The request object cannot be null.");

            var requestType = request.GetType();

            if (!_validators.TryGetValue(requestType, out var validator))
                return;

            var contextType = typeof(ValidationContext<>).MakeGenericType(requestType);
            var context = (IValidationContext)Activator.CreateInstance(contextType, request)!;

            var result = await validator.ValidateAsync(context);

            if (result.IsValid)
                return;

            throw new ValidationException(result.Errors);
        }

        protected async Task ValidateBaseEntity(Guid id)
        {
            var request = new BaseEntityDto() { Id = id };
            await ValidateRequest(request);
        }

        protected async Task<Guid> ValidateTokenUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User not authenticated");

            await ValidateBaseEntity(userId);
            return userId;
        }
    }
}
