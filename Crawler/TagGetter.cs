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

            MatchCollection matches = Regex.Matches(body.Replace("\r\n", ""), pattern);

            if (matches.Count == 0)
                return string.Empty;

            return matches[0].Groups[1].Value;
        }

        public static IEnumerable<string> ParseTableColumns(string tableColumns)
        {
            string pattern = "<td.*?>(.*?)<\\/td>";

            var matches = Regex.Matches(tableColumns, pattern);

            foreach (Match match in matches)
            {
                yield return match.Groups[1].Value;
            }
        }
    }
}