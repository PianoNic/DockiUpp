using System.Reflection;
using DockiUp.Application.Pipeline;
using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace DockiUp.Application;

public static class ModuleRegistry
{
    public static void AddApplicationModule(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediator(o =>
        {
            o.ServiceLifetime = ServiceLifetime.Scoped;
            o.Assemblies = [typeof(ModuleRegistry).Assembly];
            o.PipelineBehaviors = [typeof(FluentValidationPipelineBehavior<,>)];
        });
    }
}
