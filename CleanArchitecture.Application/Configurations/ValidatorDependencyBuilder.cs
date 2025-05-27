using CleanArchitecture.Application.Interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application.Configurations;

public class ValidatorDependencyBuilder : IDependencyBuilder
{
    public int LoadOrder => 100;

    public void Build(IServiceCollection services)
    {
        services.AddScoped(provider =>
        {
            var dict = new Dictionary<Type, IValidator>();

            var validators = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p =>
                    typeof(IValidator).IsAssignableFrom(p) &&
                    !p.IsAbstract &&
                    !p.IsGenericTypeDefinition);

            foreach (var validatorType in validators)
            {
                var modelType = validatorType.BaseType!.GetGenericArguments().First();
                var instance = (IValidator)ActivatorUtilities.CreateInstance(provider, validatorType);
                dict[modelType] = instance;
            }

            return dict;
        });
    }
}

