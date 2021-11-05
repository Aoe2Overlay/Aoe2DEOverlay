using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public delegate void PlayerStatsUpdate(Match match);
    public class PlayerStatsService
    {
        public static PlayerStatsService Instance { get; } = new ();

        public PlayerStatsUpdate Subscriber;
        
        private string baseUrl = "https://aoe2.net/api/";
        
        static PlayerStatsService()
        {
        }

        private PlayerStatsService()
        {
            MessageBus.Instance.Subscriber += message =>
            {
                if (message is WatchRecordMessage recordMessage)
                {
                    foreach (var player in recordMessage.Match.Players)
                    {
                        if(player.IsAi) continue;
                        UpdatePlayerWithState(player)
                            .GetAwaiter()
                            .OnCompleted(() =>
                            {
                                Subscriber(recordMessage.Match);
                            });
                    }
                }
            };
        }


        private async Task UpdatePlayerWithState(Player player)
        {
            await Task.WhenAll(new[]
            {
                UpdatePlayerWithRM1v1State(player),
                UpdatePlayerWithRMTeamState(player),
                UpdatePlayerWithEW1v1State(player),
                UpdatePlayerWithEWTeamState(player)
            });
        }

        private async Task UpdatePlayerWithRM1v1State(Player player)
        {
            var json = await FetchLeaderboard(player.Id, 3);
            if(json == null) return;
            var jsonArray = json["leaderboard"].Values<JObject>().ToArray();
            var jsonObject = jsonArray.Length > 0 ? jsonArray.First() : null;
            player.Country = jsonObject["country"].Value<string>() ?? "";
            MapToRaiting(jsonObject, player.RM1v1);
        }

        private async Task UpdatePlayerWithRMTeamState(Player player)
        {
            var json = await FetchLeaderboard(player.Id, 4);
            if(json == null) return;
            var jsonArray = json["leaderboard"].Values<JObject>().ToArray();
            var jsonObject = jsonArray.Length > 0 ? jsonArray.First() : null;
            player.Country = jsonObject["country"].Value<string>() ?? "";
            MapToRaiting(jsonObject, player.RMTeam);
        }

        private async Task UpdatePlayerWithEW1v1State(Player player)
        {
            var json = await FetchLeaderboard(player.Id, 13);
            if(json == null) return;
            var jsonArray = json["leaderboard"].Values<JObject>().ToArray();
            var jsonObject = jsonArray.Length > 0 ? jsonArray.First() : null;
            player.Country = jsonObject["country"].Value<string>() ?? "";
            MapToRaiting(jsonObject, player.EW1v1);
        }

        private async Task UpdatePlayerWithEWTeamState(Player player)
        {
            var json = await FetchLeaderboard(player.Id, 14);
            if(json == null) return;
            var jsonArray = json["leaderboard"].Values<JObject>().ToArray();
            var jsonObject = jsonArray.Length > 0 ? jsonArray.First() : null;
            player.Country = jsonObject["country"].Value<string>() ?? "";
            MapToRaiting(jsonObject, player.EWTeam);
        }

        private void MapToRaiting(JToken json, Raiting raiting)
        {
            raiting.Elo = json["rating"].Value<int>();
            raiting.Rank = json["rank"].Value<int>();
            raiting.Games = json["games"].Value<int>();
            raiting.Wins = json["wins"].Value<int>();
            raiting.Losses = json["losses"].Value<int>();
            raiting.LastMatchTime = json["last_match_time"].Value<int>();
            raiting.Streak = json["streak"].Value<int>();
        }
        
        private async Task<JToken> FetchLeaderboard(int profileId, int leaderboardId)
        {
            var url = $"{baseUrl}leaderboard?game=aoe2de&profile_id={profileId}&leaderboard_id={leaderboardId}";
            return await Http.FetchJSON(url);
        }

        private async Task<JToken> FetchRatinghistory(int profileId, int leaderboardId)
        {
            var url = $"{baseUrl}player/ratinghistory?game=aoe2de&profile_id={profileId}&leaderboard_id={leaderboardId}&count=1";
            return await Http.FetchJSON(url);
        }
    }
}