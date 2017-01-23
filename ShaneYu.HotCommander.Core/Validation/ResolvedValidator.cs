using System.Reflection;

using Autofac;

using ShaneYu.HotCommander.IoC;

namespace ShaneYu.HotCommander.Validation
{
    /// <summary>
    /// Resolved Vaidator
    /// </summary>
    /// <typeparam name="T">The validator type to resolve</typeparam>
    public class ResolvedValidator<T> : IPropertyValidator
        where T: IPropertyValidator
    {
        /// <summary>
        /// Validate Property
        /// </summary>
        /// <param name="property">The property to validate</param>
        /// <param name="obj">The object to validate the property on</param>
        /// <returns><c>null</c> if there was no error, otherwise the error message.</returns>
        public string Validate(PropertyInfo property, object obj)
        {
            var validator = DependencyResolver.Current.Resolve(typeof(T)) as IPropertyValidator;
            return validator?.Validate(property, obj);
        }
    }
}
