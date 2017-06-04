using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Luppa.Crawler
{
    public static class TagGetter
    {
        public static string GetValueFromTag(
            string body, 
            string tagName, 
            string id = "",
            string query = "")
        {
            string pattern = string.Empty;

            if (!string.IsNullOrEmpty(query))
                pattern = $"<{tagName} {query}.*?>(.*?)<\\/{tagName}>";
            else if (!string.IsNullOrEmpty(id)) 
                pattern = $"<{tagName} .*?id=\"{id}\".*?>(.*?)<\\/{tagName}>";

            var clearBody = body
                .Replace("\r\n", "")
                .Replace("\t", "");
            MatchCollection matches = Regex.Matches(clearBody, pattern);

            if (matches.Count == 0)
                return string.Empty;

            return matches[0].Groups[1].Value;
        }

        public static List<string> ParseTableColumns(string tableColumns)
        {
            string pattern = "<td.*?>(.*?)<\\/td>";
            string fontPattern = "<font.*?>(.*?)<\\/font>";
            var columns = new List<string>();

            var matches = Regex.Matches(tableColumns, pattern);

            foreach (Match match in matches)
            {
                var fontMatches = Regex.Matches(match.Groups[1].Value, fontPattern);

                if (fontMatches.Count > 0)
                    columns.Add(fontMatches[0].Groups[1].Value.Trim());
                else
                    columns.Add(match.Groups[1].Value.Trim());
            }

            return columns;
        }

        public static IEnumerable<string> ParseTableRows(string tableRows)
        {
            string pattern = "<tr.*?>(.*?)<\\/tr>";

            var matches = Regex.Matches(tableRows, pattern);

            foreach (Match match in matches)
            {
                yield return match.Groups[1].Value;
            }
        }
    }
}