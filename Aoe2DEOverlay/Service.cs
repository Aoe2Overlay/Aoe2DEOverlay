using System;
using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public class Service
    {
        public static Service Instance { get; } = new Service();

        public IServiceObserver observer;
        
        static Service()
        {
        }

        private Service()
        {
            timer.Elapsed += Tick;
            timer.AutoReset = true;
        }
        
        private HttpClient http = new();
        private Timer timer = new(Setting.Instance.RefreshInterval);
        private string baseUrl = "https://aoe2.net/api/";
        
        public int ProfileId = -1;
        public Data Data { get; private set; } = null;

        private class ServiceState
        {
            public bool IsPending = false;
            public bool IsLoaded = false;
            public int? MatchId = null;
        }

        private ServiceState State = new();
        
        public void Start()
        {
            Update();
            timer.Enabled = true;
        }
        
        private void Tick(Object source, ElapsedEventArgs e)
        {
            Update();
        }

        private async void Update()
        {
            if(State.IsPending) return;
            State.IsPending = true;

            timer.Interval = Setting.Instance.RefreshInterval;
            
            var data = new Data();
            var lastMatchJson = await FetchLastmatch(ProfileId);
            if (lastMatchJson == null)
            {
                State.IsPending = false;
                return;
            }
            
            var matchId = lastMatchJson["last_match"]["match_id"].Value<int>();
            if (State.MatchId != matchId || !State.IsLoaded)
            {

                var lastMatchWebJson = await FetchLastMatchWeb(ProfileId);
                if (lastMatchWebJson == null)
                {
                    State.IsPending = false;
                    return;
                }
                var webMatchId = lastMatchWebJson["id"].Value<int>();
                if (matchId != webMatchId)
                {
                    State.IsPending = false;
                    return;
                }
                State.IsLoaded = false;
                var leaderboardId = lastMatchJson["last_match"]["leaderboard_id"].Value<int>();
                data.LeaderboardId = leaderboardId;
                data.MatchModeName = ((LeaderboardType) leaderboardId).ToModeName();
                data.MatchModeShort = ((LeaderboardType) leaderboardId).ToModeShort();
                data.ServerKey = lastMatchJson["last_match"]["server"].Value<string>();
                data.ServerName = lastMatchWebJson["server"].Value<string>();
                
                var playersJson = lastMatchJson["last_match"]["players"].Values<JObject>();
                foreach (var playerJson in playersJson)
                {
                    var playerId = playerJson["profile_id"].Value<int>();
                    
                    var lbRM1v1Json = await FetchLeaderboard(playerId, 3);
                    var lbRMTeamJson = await FetchLeaderboard(playerId, 4);
                    var lbEW1v1Json = await FetchLeaderboard(playerId, 13);
                    var lbEWTeamJson = await FetchLeaderboard(playerId, 14);
                    
                    if (lbRM1v1Json == null || lbRMTeamJson == null || lbEW1v1Json == null || lbEWTeamJson == null)
                    {
                        State.IsPending = false;
                        return;
                    }

                    var player = new Player();
                    player.Id = playerId;
                    player.Slot = playerJson["slot"].Value<int>();
                    player.Color = playerJson["color"].Value<int>();
                    player.Name = playerJson["name"].Value<string>();
                    player.Civ = ((Civ) playerJson["civ"].Value<int>()).ToString();

                    var rm1v1Array = lbRM1v1Json["leaderboard"].Values<JObject>().ToArray();
                    var rmTeamArray = lbRMTeamJson["leaderboard"].Values<JObject>().ToArray();
                    var ew1v1Array = lbEW1v1Json["leaderboard"].Values<JObject>().ToArray();
                    var ewTeamArray = lbEWTeamJson["leaderboard"].Values<JObject>().ToArray();
                    
                    var rm1v1Object = rm1v1Array.Length > 0 ? rm1v1Array.First() : null;
                    var rmTeamObject = rmTeamArray.Length > 0 ? rmTeamArray.First() : null;
                    var ew1v1Object = ew1v1Array.Length > 0 ? ew1v1Array.First() : null;
                    var ewTeamObject = ewTeamArray.Length > 0 ? ewTeamArray.First() : null;

                    if (rm1v1Object != null)
                    {
                        player.Country = rm1v1Object["country"].Value<string>();
                        player.RM1v1.Elo = rm1v1Object["rating"].Value<int>();
                        player.RM1v1.Rank = rm1v1Object["rank"].Value<int>();
                        player.RM1v1.Games = rm1v1Object["games"].Value<int>();
                        player.RM1v1.Wins = rm1v1Object["wins"].Value<int>();
                        player.RM1v1.Losses = rm1v1Object["losses"].Value<int>();
                        player.RM1v1.LastMatchTime = rm1v1Object["last_match_time"].Value<int>();
                        player.RM1v1.Streak = rm1v1Object["streak"].Value<int>();
                    }

                    if (rmTeamObject != null)
                    {
                        player.Country = rmTeamObject["country"].Value<string>();
                        player.RMTeam.Elo = rmTeamObject["rating"].Value<int>();
                        player.RMTeam.Rank = rmTeamObject["rank"].Value<int>();
                        player.RMTeam.Games = rmTeamObject["games"].Value<int>();
                        player.RMTeam.Wins = rmTeamObject["wins"].Value<int>();
                        player.RMTeam.Losses = rmTeamObject["losses"].Value<int>();
                        player.RMTeam.LastMatchTime = rmTeamObject["last_match_time"].Value<int>();
                        player.RMTeam.Streak = rmTeamObject["streak"].Value<int>();
                    }


                    if (ew1v1Object != null)
                    {
                        player.Country = ew1v1Object["country"].Value<string>();
                        player.EW1v1.Elo = ew1v1Object["rating"].Value<int>();
                        player.EW1v1.Rank = ew1v1Object["rank"].Value<int>();
                        player.EW1v1.Games = ew1v1Object["games"].Value<int>();
                        player.EW1v1.Wins = ew1v1Object["wins"].Value<int>();
                        player.EW1v1.Losses = ew1v1Object["losses"].Value<int>();
                        player.EW1v1.LastMatchTime = ew1v1Object["last_match_time"].Value<int>();
                        player.EW1v1.Streak = ew1v1Object["streak"].Value<int>();
                    }

                    if (ewTeamObject != null)
                    {
                        player.Country = ewTeamObject["country"].Value<string>();
                        player.EWTeam.Elo = ewTeamObject["rating"].Value<int>();
                        player.EWTeam.Rank = ewTeamObject["rank"].Value<int>();
                        player.EWTeam.Games = ewTeamObject["games"].Value<int>();
                        player.EWTeam.Wins = ewTeamObject["wins"].Value<int>();
                        player.EWTeam.Losses = ewTeamObject["losses"].Value<int>();
                        player.EWTeam.LastMatchTime = ewTeamObject["last_match_time"].Value<int>();
                        player.EWTeam.Streak = ewTeamObject["streak"].Value<int>();
                    }

                    if (player.Country == null || player.Country == "") player.Country = "??";
                    
                    data.players.Add(player);
                }
                
                State.MatchId = matchId;
                State.IsLoaded = true;
                Data = data;
                observer?.Update(Data);
            }
            State.IsPending = false;
        }
        
        private async Task<JToken> FetchLastmatch(int profileId)
        {
            var url = $"{baseUrl}player/lastmatch?game=aoe2de&profile_id={profileId}";
            return await FetchJSON(url);
        }

        private async Task<JToken> FetchLastMatchWeb(int profileId)
        {
            var url = $"https://aoe2.net/matches/aoe2de/{profileId}?count=1";
            var json =  await FetchJSON(url);
            var data = json["data"] as JArray;
            return data?.Count > 0 ? data[0] : null;
        }

        private async Task<JToken> FetchLeaderboard(int profileId, int leaderboardId)
        {
            var url = $"{baseUrl}leaderboard?game=aoe2de&profile_id={profileId}&leaderboard_id={leaderboardId}";
            return await FetchJSON(url);
        }

        private async Task<JToken> FetchRatinghistory(int profileId, int leaderboardId)
        {
            var url = $"{baseUrl}player/ratinghistory?game=aoe2de&profile_id={profileId}&leaderboard_id={leaderboardId}&count=1";
            return await FetchJSON(url);
        }
        private async Task<string> Fetch(string url)
        {
            var response = await http.GetAsync(url);
            if (response.StatusCode != HttpStatusCode.OK) return null;
            return await response.Content.ReadAsStringAsync();
        }
        private async Task<JToken> FetchJSON(string url)
        {
            try
            {
                var content = await Fetch(url);
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