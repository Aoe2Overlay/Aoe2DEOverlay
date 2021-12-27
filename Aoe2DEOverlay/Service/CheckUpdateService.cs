using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public delegate void OnNewVersion(Version version, string url);
    public delegate void OnNoUpdates();
    public class CheckUpdateService {
        public static CheckUpdateService Instance { get; } = new ();
        
        static CheckUpdateService()
        {
        }

        private CheckUpdateService()
        {
        }

        public OnNewVersion OnNewVersion;
        public OnNoUpdates OnNoUpdates;
        

        public async void CheckRelease()
        {
            var jarray = await FetchReleases() as JArray;
            
            if(jarray == null) return;

            var hasNewVersion = false;

            foreach (var json in jarray)
            {
                var name = json["name"].Value<string>();
                var tag = json["tag_name"].Value<string>().TrimStart('v');
                if(tag != name) continue;

                var isPrerelease = json["prerelease"].Value<bool>();
                if(isPrerelease && !Setting.Instance.Update.Prerelease) continue;

                var version = new Version(tag);
                if(Metadata.Version >= version) continue;

                var now = DateTime.Now;
                var date = json["published_at"].Value<DateTime>();
                var delta = (now - date).TotalHours;
                var waiting = Setting.Instance.Update.Waiting;
                if(delta < 1 && Setting.Instance.Update.Waiting) continue;
                var url = ParseDownloadUrl(json);
                if(url != null)
                {
                    hasNewVersion = true;
                    OnNewVersion(version, url);
                }
                break;
            }
            if(!hasNewVersion) OnNoUpdates();
        }

        public string ParseDownloadUrl(JToken json)
        {
            var assets = json["assets"].Values<JObject>();
            foreach (var asset in assets)
            {
                var name = asset["name"]?.Value<string>() ?? "";
                var platform = Metadata.platform.ToString();
                if(name == $"{platform}.zip")
                {
                    return asset["browser_download_url"]?.Value<string>() ?? "";
                }
            }

            return null;
        }

        private async Task<JToken> FetchReleases()
        {
            var url = "https://api.github.com/repos/kickass-panda/Aoe2DEOverlay/releases";
            var headers = new Dictionary<string, string>()
            {
                { "User-Agent", "request" },
            };
            return await Http.FetchJSON(url, headers);
        }
    }
}