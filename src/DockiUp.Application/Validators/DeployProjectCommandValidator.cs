using DockiUp.Application.Commands;
using DockiUp.Application.Validators.Helpers;
using DockiUp.Domain.Enums;
using FluentValidation;

namespace DockiUp.Application.Validators
{
    public class DeployProjectCommandValidator : AbstractValidator<DeployProjectCommand>
    {
        public DeployProjectCommandValidator()
        {
            RuleFor(command => command.SetupContainerDto.ProjectName)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(100).WithMessage("Project name must not exceed 100 characters.");

            RuleFor(command => command.SetupContainerDto.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

            RuleFor(command => command.SetupContainerDto.ProjectOrigin)
                .IsValidEnum().WithMessage("Invalid project origin type.");

            When(command => command.SetupContainerDto.ProjectOrigin == ProjectOriginType.Git, () =>
            {
                RuleFor(command => command.SetupContainerDto.GitUrl)
                    .NotNull().NotEmpty().WithMessage("Git URL is required when Project Origin is 'Git'.");
                RuleFor(command => command.SetupContainerDto.Compose)
                    .NotNull().NotEmpty().WithMessage("Compose content is required when Project Origin is 'Git'.");
            });

            When(command => command.SetupContainerDto.ProjectOrigin == ProjectOriginType.Compose, () =>
            {
                RuleFor(command => command.SetupContainerDto.Compose)
                    .NotNull().NotEmpty().WithMessage("Compose content is required when Project Origin is 'Compose'.");
            });

            When(command => command.SetupContainerDto.ProjectOrigin == ProjectOriginType.Import, () =>
            {
                RuleFor(command => command.SetupContainerDto.Path)
                    .NotNull().NotEmpty().WithMessage("Path is required when Project Origin is 'Import'.");
            });

            RuleFor(command => command.SetupContainerDto.GitUrl)
                .MaximumLength(2000).WithMessage("Git URL must not exceed 2000 characters.")
                .When(command => !string.IsNullOrWhiteSpace(command.SetupContainerDto.GitUrl));

            RuleFor(command => command.SetupContainerDto.Compose)
                .MaximumLength(10000).WithMessage("Compose content must not exceed 10000 characters.")
                .When(command => !string.IsNullOrWhiteSpace(command.SetupContainerDto.Compose));

            RuleFor(command => command.SetupContainerDto.Path)
                .MaximumLength(500).WithMessage("Path must not exceed 500 characters.")
                .When(command => !string.IsNullOrWhiteSpace(command.SetupContainerDto.Path));

            RuleFor(command => command.SetupContainerDto.ProjectUpdateMethod)
                .IsValidEnum().WithMessage("Invalid project update method.");

            When(command => command.SetupContainerDto.ProjectUpdateMethod == ProjectUpdateMethod.Webhook, () =>
            {
                RuleFor(command => command.SetupContainerDto.WebhookUrl)
                    .NotNull().NotEmpty().WithMessage("Webhook URL is required when Project Update Method is 'Webhook'.");
            });

            When(command => command.SetupContainerDto.ProjectUpdateMethod == ProjectUpdateMethod.Periodically, () =>
            {
                RuleFor(command => command.SetupContainerDto.PeriodicIntervalInMinutes)
                    .NotNull().WithMessage("Periodic interval is required when Project Update Method is 'Periodically'.")
                    .GreaterThan(0).WithMessage("Periodic interval must be greater than 0.");
            });

            RuleFor(command => command.SetupContainerDto.WebhookUrl)
                .MaximumLength(2000).WithMessage("Webhook URL must not exceed 2000 characters.")
                .When(command => !string.IsNullOrWhiteSpace(command.SetupContainerDto.WebhookUrl));

            RuleFor(command => command.SetupContainerDto.PeriodicIntervalInMinutes)
                .GreaterThan(0).WithMessage("Periodic interval must be greater than 0.")
                .When(command => command.SetupContainerDto.PeriodicIntervalInMinutes.HasValue &&
                                 command.SetupContainerDto.ProjectUpdateMethod != ProjectUpdateMethod.Periodically);
        }
    }
}