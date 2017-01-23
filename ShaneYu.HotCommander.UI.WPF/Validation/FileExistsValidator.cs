using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;

using ShaneYu.HotCommander.Validation;

namespace ShaneYu.HotCommander.UI.WPF.Validation
{
    /// <summary>
    /// File Exists Validator
    /// </summary>
    public class FileExistsValidator : IFileExistsValidator
    {
        /// <summary>
        /// Validate Property
        /// </summary>
        /// <param name="property">The property to validate</param>
        /// <param name="obj">The object to validate the property on</param>
        /// <returns><c>null</c> if there was no error, otherwise the error message.</returns>
        public string Validate(PropertyInfo property, object obj)
        {
            if (property.PropertyType != typeof (string))
            {
                throw new ArgumentException(@"This validator only works with properties of type 'string'.", nameof(property));
            }

            var value = (string) property.GetValue(obj);
            var isRequired = property.GetCustomAttribute<RequiredAttribute>() != null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                if (!File.Exists(value))
                    return "The specified file does not exist.";
            }
            else if (isRequired)
            {
                return "This field is required, a file must be specified.";
            }

            return null;
        }
    }
}
