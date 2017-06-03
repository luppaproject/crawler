using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Luppa.Crawler
{
    public static class HtmlGetter
    {
        public static async Task<string> GetBodyFrom(string url)
        {
            using(var httpClient = new HttpClient())
            {
                var parameters = new Dictionary<string, string>();

                var response = await httpClient.GetAsync(url);
                var contents = await response.Content.ReadAsStringAsync();

                File.AppendAllText("C:\\teste\\teste.txt", contents);

                return contents;
            }
        }
    }
}