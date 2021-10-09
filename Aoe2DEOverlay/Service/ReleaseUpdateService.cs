using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public class ReleaseUpdateService
    {
        
        public static ReleaseUpdateService Instance { get; } = new ReleaseUpdateService();

        public IReleaseObserver Observer;
        
        static ReleaseUpdateService()
        {
        }

        private ReleaseUpdateService()
        {
        }

        public async void   CheckRelease()
        {
            var jarray = await FetchReleases() as JArray;
            
            if(jarray == null) return;

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
                
                Observer?.UpdateAvailable(version);
                    
            }
            
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