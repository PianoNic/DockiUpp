using FluentValidation;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace DockiUp.Application.Pipeline
{
    public sealed class FluentValidationPipelineBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
        where TMessage : IMessage
    {
        private readonly IServiceProvider _serviceProvider;

        public FluentValidationPipelineBehavior(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
        {
            var validator = _serviceProvider.GetService<IValidator<TMessage>>();
            if (validator is not null)
            {
                var result = await validator.ValidateAsync(message, cancellationToken);
                if (!result.IsValid)
                    throw new ValidationException(result.Errors);
            }

            return await next(message, cancellationToken);
        }
    }
}
