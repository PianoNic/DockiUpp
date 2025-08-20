using FluentValidation;

namespace DockiUp.Application.Validators.Helpers
{
    /// <summary>
    /// Provides utility methods for validating enum values.
    /// </summary>
    public static class EnumValidationHelper
    {
        /// <summary>
        /// Defines a validator that ensures the property value is a defined member of its enum type.
        /// </summary>
        /// <typeparam name="T">The type of the object being validated.</typeparam>
        /// <typeparam name="TProperty">The type of the property being validated, which must be an enum.</typeparam>
        /// <param name="ruleBuilder">The rule builder.</param>
        /// <returns>An <see cref="IRuleBuilderOptions{T, TProperty}"/> for chaining.</returns>
        public static IRuleBuilderOptions<T, TProperty> IsValidEnum<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
            where TProperty : struct, Enum // Constraint to ensure TProperty is an enum
        {
            // Use the Must method with Enum.IsDefined, as IsInEnumValidator is internal.
            return ruleBuilder.Must(value => Enum.IsDefined(typeof(TProperty), value));
        }
    }
}
