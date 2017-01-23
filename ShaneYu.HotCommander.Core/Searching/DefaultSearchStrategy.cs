using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Commands;

namespace ShaneYu.HotCommander.Searching
{
    public class DefaultSearchStrategy : ISearchStrategy<IHotCommand<IHotCommandConfiguration>>
    {
        /// <summary>
        /// Search Commands
        /// </summary>
        /// <param name="allCommands">The commands to search through</param>
        /// <param name="searchTerm">The search term to use</param>
        /// <param name="excludeInvariant">Whether to exclude invariant commands from the search</param>
        /// <param name="includeDisabled">Whether to include disabled commands from the search</param>
        /// <returns>All of the found commands</returns>
        public IEnumerable<IHotCommand<IHotCommandConfiguration>> Search(
            IEnumerable<IHotCommand<IHotCommandConfiguration>> allCommands,
            string searchTerm,
            bool excludeInvariant = false,
            bool includeDisabled = false)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Enumerable.Empty<IHotCommand<IHotCommandConfiguration>>();

            var pattern1 =
                $"^{string.Concat(searchTerm.ToUpper().Select(x => $"({Regex.Escape(x.ToString())})[\\.:a-z0-9\\s]*"))}.*$";
            var regex1 = new Regex(pattern1);

            var pattern2 = $"^((?<match>{Regex.Escape(searchTerm)})|.*(?<match>{Regex.Escape(searchTerm)}).*).*$";
            var regex2 = new Regex(pattern2, RegexOptions.IgnoreCase);

            // Find all commands where the name matches the search term.
            var foundCommands =
                allCommands.Where(x => regex1.IsMatch(x.Configuration.Name) || regex2.IsMatch(x.Configuration.Name));

            if (excludeInvariant)
            {
                // We've decided to exclude invariant commands, so lets remove these from the found commands.
                foundCommands =
                    foundCommands.Where(
                        x => !x.GetType().GetCustomAttributes(typeof (InternalHotCommandAttribute), false).Any());
            }

            if (!includeDisabled)
            {
                // We've decided NOT to include disabled commands, so lets remove these from the found commands.
                foundCommands = foundCommands.Where(x => x.Configuration.IsEnabled);
            }

            return foundCommands;
        }
    }
}
