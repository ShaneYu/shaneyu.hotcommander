using System.Collections.Generic;

using ShaneYu.HotCommander.Commands;

namespace ShaneYu.HotCommander.Searching
{
    /// <summary>
    /// Command Search Strategy Interface
    /// </summary>
    public interface ISearchStrategy<out T>
    {
        /// <summary>
        /// Search Commands
        /// </summary>
        /// <param name="allCommands">The commands to search through</param>
        /// <param name="searchTerm">The search term to use</param>
        /// <param name="excludeInvariant">Whether to exclude invariant commands from the search</param>
        /// <param name="includeDisabled">Whether to include disabled commands from the search</param>
        /// <returns>All of the found commands in the desired return type</returns>
        IEnumerable<T> Search(
            IEnumerable<IHotCommand<IHotCommandConfiguration>> allCommands, 
            string searchTerm,
            bool excludeInvariant = false, 
            bool includeDisabled = false);
    }
}
