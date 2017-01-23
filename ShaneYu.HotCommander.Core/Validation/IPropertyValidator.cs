using System.Reflection;

namespace ShaneYu.HotCommander.Validation
{
    /// <summary>
    /// Property Validator Interface
    /// </summary>
    public interface IPropertyValidator
    {
        /// <summary>
        /// Validate Property
        /// </summary>
        /// <param name="property">The property to validate</param>
        /// <param name="obj">The object to validate the property on</param>
        /// <returns><c>null</c> if there was no error, otherwise the error message.</returns>
        string Validate(PropertyInfo property, object obj);
    }
}
