using System.Linq;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands;

namespace ShaneYu.HotCommander.Extensions
{
    /// <summary>
    /// Command Extensions
    /// </summary>
    public static class HotCommandExtensions
    {
        /// <summary>
        /// Checks if this is an internal command.
        /// </summary>
        /// <param name="hotCommand">The command to check</param>
        /// <returns><c>true</c> if the command is internal, otherwise <c>false</c></returns>
        public static bool IsInternal(this IHotCommand<IHotCommandConfiguration> hotCommand)
        {
            return hotCommand.GetType().GetCustomAttributes(typeof(InternalHotCommandAttribute), false).Any();
        }
    }
}
