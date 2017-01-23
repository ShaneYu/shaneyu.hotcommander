using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ShaneYu.HotCommander.Attributes
{
    /// <summary>
    /// Cannot Be Empty Attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class CannotBeEmptyAttribute : RequiredAttribute
    {
        /// <summary>
        /// Checks that the value of the required data field is not empty.
        /// </summary>
        /// <returns>
        /// true if validation is successful; otherwise, false.
        /// </returns>
        /// <param name="value">The data field value to validate.</param><exception cref="T:System.ComponentModel.DataAnnotations.ValidationException">The data field value was null.</exception>
        public override bool IsValid(object value)
        {
            var list = value as IEnumerable;
            return list != null && list.GetEnumerator().MoveNext();
        }
    }
}
