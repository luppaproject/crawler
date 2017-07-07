using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Luppa.Helpers
{
    public static class HtmlGetter
    {
        public static async Task<string> GetBodyFrom(string url)
        {
            using (var httpClient = new HttpClient())
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
            Dictionary<string, string> formUrlContent)
        {
            using (var httpClient = new HttpClient())
            {
                var formList = new List<string>();

                foreach (var item in formUrlContent)
                    formList.Add($"{item.Key}={WebUtility.UrlEncode(item.Value)}");

                var formBody = string.Join("&", formList);
                var form = new StringContent(formBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(url, form);
                var contents = await response.Content.ReadAsStringAsync();

                contents = WebUtility.HtmlDecode(contents);

                return contents;
            }
        }

        public static async Task<string> PostAndGetRedirectUrl(
            string url, 
            Dictionary<string, string> formUrlContent)
        {
            using (var httpClient = new HttpClient())
            {
                var formList = new List<string>();

                foreach (var item in formUrlContent)
                    formList.Add($"{item.Key}={WebUtility.UrlEncode(item.Value)}");

                var formBody = string.Join("&", formList);
                var form = new StringContent(formBody, Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await httpClient.PostAsync(url, form);
                var contents = await response.Content.ReadAsStringAsync();
                var redirectUrl = response.Headers.GetValues("Location").First();

                return redirectUrl;
            }
        }
    }
}