using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public static class Http
    {
        private static HttpClient http = new();
        public static async Task<string> Fetch(string url, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            if(headers != null) foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            var response = await http.SendAsync(request);
            if (response.StatusCode != HttpStatusCode.OK) return null;
            return await response.Content.ReadAsStringAsync();
        }
        public static async Task<JToken> FetchJSON(string url, Dictionary<string, string> headers = null)
        {
            try
            {
                var content = await Fetch(url, headers);
                content = content.Trim();
                if (content.StartsWith("{") && content.EndsWith("}"))
                {
                    return JObject.Parse(content);
                }

                if (content.StartsWith("[") && content.EndsWith("]"))
                {
                    return JArray.Parse(content);
                }
            }
            catch
            {
                // ignored
            }
            return null;
        }
        
    }
}