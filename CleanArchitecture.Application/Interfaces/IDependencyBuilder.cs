using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application.Interfaces;

public interface IDependencyBuilder
{
    int LoadOrder { get; }
    void Build(IServiceCollection services);
}
