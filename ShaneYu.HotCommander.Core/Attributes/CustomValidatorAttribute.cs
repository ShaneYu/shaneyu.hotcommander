using System;

using ShaneYu.HotCommander.Validation;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Custom Validation Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class CustomValidatorAttribute : Attribute
    {
        private readonly Type _validatorType;

        /// <summary>
        /// Gets the validator.
        /// </summary>
        public IPropertyValidator Validator
        {
            get
            {
                if (_validatorType == null) return null;

                if (typeof(ResolvedValidator<IPropertyValidator>).IsAssignableFrom(_validatorType))
                    return (ResolvedValidator<IPropertyValidator>) Activator.CreateInstance(_validatorType);

                return (IPropertyValidator) Activator.CreateInstance(_validatorType);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validatorType">The validator type</param>
        public CustomValidatorAttribute(Type validatorType)
        {
            if (!typeof(IPropertyValidator).IsAssignableFrom(validatorType))
            {
                throw new ArgumentException($"The validator type must implement '{nameof(IPropertyValidator)}'", nameof(validatorType));
            }

            _validatorType = validatorType;
        }
    }
}
