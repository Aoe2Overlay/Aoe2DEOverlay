using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Documents;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public delegate void MatchStateUpdate(Match match);

    public class MatchStateService
    {
        public static MatchStateService Instance { get; } = new ();

        public MatchStateUpdate Subscriber;

        private string baseUrl = "https://aoe2.net/api/";
        
        static MatchStateService()
        {
        }

        private MatchStateService()
        {
            MessageBus.Instance.Subscriber += message =>
            {
                if (message is WatchRecordMessage recordMessage)
                {
                    var match = recordMessage.Match;
                    UpdateMatchWithState(match);
                }
            };
        }

        private void UpdateMatchWithState(Match match, int count = 0)
        {
            if (!match.IsMultiplayer) return;
            var profileId = Setting.Instance.ProfileId;
            var isValid = profileId > 0;
            if (isValid)
            {
                var json = FetchLastmatchApi(profileId).GetAwaiter().GetResult();
                isValid = UpdateMatchWithApiJson(match, json);
            }
            if (isValid)
            {
                var json = FetchLastMatchWeb(profileId).GetAwaiter().GetResult();
                isValid = UpdateMatchWithWebJson(match, json);
            }
            if (isValid)
            {
                Subscriber(match);
            }
            else
            {
                count += 1;
                if (count < 10000)
                {
                    var timer = new Timer(5000);
                    timer.AutoReset = false;
                    timer.Elapsed += (sender, args) =>
                    {
                        timer.Stop();
                        UpdateMatchWithState(match, count + 1);
                    };
                    timer.Start();
                }
            }
        }

        private bool UpdateMatchWithApiJson(Match match, JToken json)
        {
            if(!ValidateMatch(match, json["last_match"])) return false;
            match.ServerKey = json["last_match"]["server"].Value<string>();
            return true;
        }

        private bool UpdateMatchWithWebJson(Match match, JToken json)
        {
            if(!ValidateMatch(match, json)) return false;
            match.ServerName = json["server"].Value<string>();
            return true;
        }

        private bool ValidateMatch(Match match, JToken json)
        {
            var playersJson = json["players"].Values<JObject>();
            var started = json["started"].Value<uint>();
            var pIds = new int[8];
            foreach (var playerJson in playersJson)
            {
                var slot = playerJson["slot"].Value<int>();
                var profileToken = playerJson["profile_id"];
                if (profileToken == null) profileToken = playerJson["profileId"];
                var id = 0;
                if(profileToken != null)  id = profileToken.Value<int>();
                pIds[slot-1] = id;
            }
            return ValidateMatch(match, started,
                pIds[0], pIds[1],
                pIds[2], pIds[3],
                pIds[4], pIds[5],
                pIds[6], pIds[7]);
        }

        private bool ValidateMatch(Match match, uint started, int p1Id, int p2Id, int p3Id, int p4Id, int p5Id, int p6Id, int p7Id, int p8Id)
        {
            var result = Math.Abs(match.Started - started);
            if (result > 10) return false;
            foreach (var player in match.Players)
            {
                if (player.Slot == 1 && player.Id != p1Id) return false;
                if (player.Slot == 2 && player.Id != p2Id) return false;
                if (player.Slot == 3 && player.Id != p3Id) return false;
                if (player.Slot == 4 && player.Id != p4Id) return false;
                if (player.Slot == 5 && player.Id != p5Id) return false;
                if (player.Slot == 6 && player.Id != p6Id) return false;
                if (player.Slot == 7 && player.Id != p7Id) return false;
                if (player.Slot == 8 && player.Id != p8Id) return false;
            }
            return true;
        }
        
        private async Task<JToken> FetchLastmatchApi(int profileId)
        {
            var url = $"{baseUrl}player/lastmatch?game=aoe2de&profile_id={profileId}";
            return await Http.FetchJSON(url);
        }

        private async Task<JToken> FetchLastMatchWeb(int profileId)
        {
            var url = $"https://aoe2.net/matches/aoe2de/{profileId}?count=1";
            var json =  await Http.FetchJSON(url);
            var data = json["data"] as JArray;
            return data?.Count > 0 ? data[0] : null;
        }
    }
}