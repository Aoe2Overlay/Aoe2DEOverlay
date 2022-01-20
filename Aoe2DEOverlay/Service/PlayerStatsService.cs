using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public delegate void OnPlayerUpdate(Match match);
    public class PlayerStatsService
    {
        public static PlayerStatsService Instance { get; } = new ();

        public OnPlayerUpdate OnPlayerUpdate;
        
        private string baseUrl = "https://aoe2.net/api/";
        
        static PlayerStatsService()
        {
        }

        private PlayerStatsService()
        {
            WatchRecordService.Instance.OnMatchUpdate += match =>
            {
                foreach (var player in match.Players)
                {
                    if(player.IsAi) continue;
                    UpdatePlayerWithState(player)
                        .GetAwaiter()
                        .OnCompleted(() =>
                        {
                            OnPlayerUpdate(match);
                        });
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
            await UpdatePlayerWithEWTeamState(player, 3, player.RM1v1);
        }

        private async Task UpdatePlayerWithRMTeamState(Player player)
        {
            await UpdatePlayerWithEWTeamState(player, 4, player.RMTeam);
        }

        private async Task UpdatePlayerWithEW1v1State(Player player)
        {
            await UpdatePlayerWithEWTeamState(player, 13, player.EW1v1);
        }

        private async Task UpdatePlayerWithEWTeamState(Player player)
        {
            await UpdatePlayerWithEWTeamState(player, 14, player.EWTeam);
        }

        private async Task UpdatePlayerWithEWTeamState(Player player, int leaderboardId, Raiting raiting)
        {
            var json = await FetchLeaderboard(player.Id, leaderboardId);
            if(json == null) return;
            var jsonArray = json["leaderboard"].Values<JObject>().ToArray();
            var jsonObject = jsonArray.Length > 0 ? jsonArray.First() : null;
            if (jsonObject != null)
            {
                raiting.IsActive = true;
                player.Country = jsonObject["country"].Value<string>() ?? "";
                MapToRaiting(jsonObject, raiting);
            }
            else
            {
                raiting.IsActive = false;
                json = await FetchRatinghistory(player.Id, leaderboardId);
                if(json == null) return;
                jsonArray = json.Values<JObject>().ToArray();
                jsonObject = jsonArray.Length > 0 ? jsonArray.First() : null;
                if (jsonObject == null) return;
                MapHistoryToRaiting(jsonObject, raiting);
            }
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

        private void MapHistoryToRaiting(JToken json, Raiting raiting)
        {
            raiting.Elo = json["rating"].Value<int>();
            raiting.Wins = json["num_wins"].Value<int>();
            raiting.Losses = json["num_losses"].Value<int>();
            raiting.Games = raiting.Wins + raiting.Losses;
            raiting.LastMatchTime = json["timestamp"].Value<int>();
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