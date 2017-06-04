using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
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
                contents = WebUtility.HtmlDecode(contents);

                return contents;
            }
        }

        public static async Task<string> PostBodyFrom(
            string url, 
            string viewState,
            string eventTarget,
            string eventArgument)
        {
            using(var httpClient = new HttpClient())
            {
                // var parameters = $"__VIEWSTATE: {viewState}\r\n";
                // parameters += $"__EVENTTARGET: {eventTarget}\r\n";
                // parameters += $"__EVENTARGUMENT: {eventArgument}";
                var parameters = new Dictionary<string, string>
                {
                    ["__VIEWSTATE"] = viewState,
                    ["__EVENTTARGET"] = eventTarget,
                    ["__EVENTARGUMENT"] = eventArgument,
                };

                var form = new FormUrlEncodedContent(parameters);

                var response = await httpClient.PostAsync(url, form);
                var contents = await response.Content.ReadAsStringAsync();
                contents = WebUtility.HtmlDecode(contents);

                return contents;
            }
        }
    }
}