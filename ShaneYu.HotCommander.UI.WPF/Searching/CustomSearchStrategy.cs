using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;

using ShaneYu.HotCommander.Commands;
using ShaneYu.HotCommander.Attributes;
using ShaneYu.HotCommander.Searching;

namespace ShaneYu.HotCommander.UI.WPF.Searching
{
    public class CustomSearchStrategy : ISearchStrategy<TextBlock>
    {
        public IEnumerable<TextBlock> Search(IEnumerable<IHotCommand<IHotCommandConfiguration>> allCommands, string searchTerm, bool excludeInvariant = false, bool includeDisabled = false)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<TextBlock>();
            }

            // Filter out internal and disabled commands if specified...
            allCommands =
                allCommands.Where(
                    x =>
                        (!excludeInvariant || !x.GetType().GetCustomAttributes(typeof(InternalHotCommandAttribute), false).Any()) &&
                        (includeDisabled
                         || x.Configuration.IsEnabled));

            var textBlocks = new List<TextBlock>();

            var pattern1 = $"^{string.Concat(searchTerm.ToUpper().Select(x => $"({Regex.Escape(x.ToString())})[\\.:\\(\\)\\[\\]<>{{}}a-z0-9\\s]*"))}.*$";
            var regex1 = new Regex(pattern1);

            var pattern2 = $"^((?<match>{Regex.Escape(searchTerm)})|.*(?<match>{Regex.Escape(searchTerm)}).*).*$";
            var regex2 = new Regex(pattern2, RegexOptions.IgnoreCase);

            foreach (var command in allCommands)
            {
                var match = regex1.Match(command.Configuration.Name);

                if (match.Success)
                {
                    var textBlock = new TextBlock { Tag = command.Configuration.Id };

                    for (var i = 1; i < match.Groups.Count; i++)
                    {
                        var group = match.Groups[i];

                        var matchedText = group.Value;

                        var bold = new Bold();
                        bold.Inlines.Add(matchedText);
                        textBlock.Inlines.Add(bold);

                        if (i == match.Groups.Count - 1)
                        {
                            textBlock.Inlines.Add(command.Configuration.Name.Substring(group.Index + group.Length));
                        }
                        else
                        {
                            var nextGroup = match.Groups[i + 1];
                            var unmatchedText = command.Configuration.Name.Substring(group.Index + group.Length, nextGroup.Index - (group.Index + group.Length));

                            textBlock.Inlines.Add(unmatchedText);
                        }
                    }

                    textBlocks.Add(textBlock);
                    continue;
                }

                match = regex2.Match(command.Configuration.Name);

                if (match.Success)
                {
                    var textBlock = new TextBlock { Tag = command.Configuration.Id };

                    if (match.Groups["match"].Index > 0)
                        textBlock.Inlines.Add(command.Configuration.Name.Substring(0, match.Groups["match"].Index));

                    var bold = new Bold();
                    bold.Inlines.Add(match.Groups["match"].Value);
                    textBlock.Inlines.Add(bold);

                    if (match.Groups["match"].Index + match.Groups["match"].Length < command.Configuration.Name.Length)
                        textBlock.Inlines.Add(command.Configuration.Name.Substring(match.Groups["match"].Index + match.Groups["match"].Length));

                    textBlocks.Add(textBlock);
                }
            }

            return textBlocks;
        }
    }
}
