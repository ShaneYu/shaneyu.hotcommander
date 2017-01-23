using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

using ShaneYu.HotCommander.Attributes;

namespace ShaneYu.HotCommander.Helpers
{
    /// <summary>
    /// Validation Helper
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Performs a validation check on the <paramref name="obj"/>
        /// </summary>
        /// <param name="obj">The object to perform validation checks on</param>
        /// <returns><c>true</c> if the object is valid, otherwise false.</returns>
        public static bool ValidateObject(object obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj, null, null);

            if (!Validator.TryValidateObject(obj, validationContext, validationResults, true) ||
                validationResults.Count > 0)
            {
                return false;
            }

            return
                !obj.GetType()
                    .GetProperties()
                    .Any(
                        pi =>
                            pi.CanRead &&
                            pi.GetCustomAttributes<CustomValidatorAttribute>()
                                .Select(customValidatorAttr => customValidatorAttr.Validator.Validate(pi, obj))
                                .Any(errorMessage => !string.IsNullOrWhiteSpace(errorMessage)));
        }

        /// <summary>
        /// Gets the first validation error message for a property
        /// </summary>
        /// <param name="obj">The object instance to validate the property on</param>
        /// <param name="propertyName">The name of the property to validate</param>
        /// <returns><c>null</c> if there are no validation errors, otherwise the first validation error message</returns>
        public static string GetValidationErrorMessageForProperty(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return GetValidationErrorMessageForProperty(obj, propertyInfo);
        }

        /// <summary>
        /// Gets the first validation error message for a property
        /// </summary>
        /// <param name="obj">The object instance to validate the propert on</param>
        /// <param name="propertyInfo">The property to validate</param>
        /// <returns><c>null</c> if there are no validation errors, otherwise the first validation error message</returns>
        public static string GetValidationErrorMessageForProperty(object obj, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null || obj == null) return null;

            var validationContext = new ValidationContext(obj, null, null)
            {
                MemberName = propertyInfo.Name
            };

            var propertyValue = propertyInfo.GetValue(obj);
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateProperty(propertyValue, validationContext, validationResults);

            if (!isValid)
                return validationResults.FirstOrDefault()?.ErrorMessage;

            return
                propertyInfo.GetCustomAttributes<CustomValidatorAttribute>().Select(
                    customValidatorAttr => customValidatorAttr.Validator.Validate(propertyInfo, obj))
                    .FirstOrDefault(errorMessage => !string.IsNullOrWhiteSpace(errorMessage));
        }
    }
}
