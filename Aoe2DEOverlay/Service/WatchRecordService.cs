using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using ReadAoe2Recrod;

namespace Aoe2DEOverlay
{
    public delegate void OnMatchUpdate(Match match);
    public delegate void OnRecordReadError(Match match);
    public class WatchRecordService
    {
        private string homePath = Environment.GetEnvironmentVariable("HOMEPATH");
        private string basePath;
        private string filter = "*.aoe2record";
        private string lastFile = "";

        private List<FileSystemWatcher> watchers = new();

        public OnMatchUpdate OnMatchUpdate;
        public OnRecordReadError OnRecordReadError;
        
        public static WatchRecordService Instance { get; } = new ();

        WatchRecordService()
        {
            basePath = $"{homePath}\\Games\\Age of Empires 2 DE";
            Start();
        }

        private void Start()
        {
            var saveGamePaths = new DirectoryInfo(basePath)
                .GetDirectories()
                .Where( dir => dir.GetDirectories().Where(subDir => subDir.Name == "savegame").Count() > 0)
                .Where( dir => dir.Name != "0")
                .Select( dir => $"{dir.FullName}\\savegame")
                .ToList();
            FileInfo? latest = null;
            saveGamePaths.ForEach(saveGamePath =>
            {
                var watcher = new FileSystemWatcher();
                watcher.Path = saveGamePath;
                watcher.NotifyFilter = NotifyFilters.CreationTime |  
                                       NotifyFilters.LastWrite |   
                                       NotifyFilters.Size;
                watcher.Changed += OnFileChanged;
                watcher.Filter = filter;  
                watcher.EnableRaisingEvents = true; 
                watchers.Add(watcher);

                var file = new DirectoryInfo(saveGamePath)
                    .GetFiles()
                    .OrderByDescending(f => f.LastWriteTime)
                    .FirstOrDefault();

                if (latest == null) latest = file;
                if (latest != null && latest.LastWriteTime < file.LastWriteTime) latest = file;
            });

            if (latest != null)
            {
                var file =  latest.FullName;
                
                var t1970 = new DateTime(1970, 1, 1);
                var before = (uint)File.GetLastWriteTime(file).Subtract(t1970).TotalSeconds;
                var timer = new Timer(1000);
                timer.AutoReset = false;
                timer.Elapsed += (sender, args) =>
                {
                    var after = (uint)File.GetLastWriteTime(file).Subtract(t1970).TotalSeconds;
                    //if(before < after) OnChanged(file);
                    OnChanged(file); // always show latest
                };
                timer.Start();
            }
        }
        
        private void OnFileChanged(object source, FileSystemEventArgs e)  
        {  
            var file = new FileInfo(e.FullPath).FullName;
            OnChanged(file);
        } 
        
        private void OnChanged(string file)  
        {  
            var saveGamePath = string.Join('\\', file.Split('\\').SkipLast(1));
            file = new DirectoryInfo(saveGamePath)
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault()?.FullName;
            
            if (file == lastFile) return; // ignore
            lastFile = file;
            try
            {
                var record = Read(file);
                var match = MatchFromRecord(record);
                match.SteamId = SteamIdFromPath(file);
                if (match.SteamId == null)
                {
                    match.ProfileId = DetectProfileId(file);
                }
                OnMatchUpdate(match);
            }
            catch (Exception)
            {
                lastFile = "";
                var steamId = SteamIdFromPath(file) ?? 0;
                var match = new Match();
                match.SteamId = steamId;
                match.Started = StartedFromFile(file);
                match.IsMultiplayer = IsMultiplayerFromPath(file);
                OnRecordReadError(match);
            }
        } 
        
        private Match MatchFromRecord(Aoe2Record record)
        {
            var match = new Match();
            match.Started = record.Started;
            match.Difficulty = record.Difficulty;
            match.IsMultiplayer = record.IsMultiplayer;
            match.IsRanked = record.IsRanked;
            match.MapType = record.MapType;
            match.MapName = record.MapName;
            match.GameTypeName = record.GameTypeName;
            match.GameTypeShort = record.GameTypeShort;
            match.HasAI = record.HasAI;
            foreach (var recordPlayer in record.Players)
            {
                var player = new Player();
                player.Civ = recordPlayer.Civ;
                player.Name = recordPlayer.Name;
                player.Color = recordPlayer.Color;
                player.Id = (int)recordPlayer.ProfileId;
                player.Slot = recordPlayer.Slot;
                match.Players.Add(player);
            }
            return match;
        }
        
        private uint? DetectProfileId(string file)
        {
            var saveGamePath = string.Join('\\', file.Split('\\').SkipLast(1));
            var saveGameFiles = new DirectoryInfo(saveGamePath).GetFiles();

            var counters = new Dictionary<uint, uint>(); // <profileId, count>
            foreach (var saveGameFile in saveGameFiles)
            {
                if(counters.Count == 1) break;
                try
                {
                    var record = Read(saveGameFile.FullName);
                    if (!record.IsMultiplayer)
                    {
                        counters.Clear();
                        var id = record.Players.ToList().Find(player => !player.IsAi)?.ProfileId ?? 0;
                        counters[id] = 1;
                        break;
                    }
                    foreach (var player in record.Players)
                    {
                        if (player.IsAi) continue;
                        if (counters.ContainsKey(player.ProfileId))
                        {
                            counters[player.ProfileId] += 1;
                        }
                        else
                        {
                            counters[player.ProfileId] = 1;
                        }
                    }

                    uint maxCount = 0;
                    counters.ToList().ForEach(pair => maxCount = pair.Value > maxCount ? pair.Value : maxCount);
                    foreach (var pair in new Dictionary<uint, uint>(counters) )
                    {
                        if (pair.Value < maxCount)
                        {
                            counters.Remove(pair.Key);
                        }
                    }
                }
                catch{continue;}
            }

            uint? profileId = null;

            if (counters.Count == 1)
            {
                profileId = counters.First().Key;
                // TODO: save mapping into settings like:
                // Setting.Instance.Profiles = {IdFromPath(file): march.ProfileId}
            }

            return profileId == 0 ? null : profileId;
        }
        private ulong? IdFromPath(string file)
        {
            var path = file.Split("\\savegame")[0].Split("\\");
            var id = Convert.ToUInt64(path[^1]);
            return id > 0 ? id : null;
        }

        private ulong? SteamIdFromPath(string file)
        {
            var path = file.Split("\\savegame")[0].Split("\\");
            var steamStr = path[^1];
            if (steamStr.Length != 17) return null;
            var steamId = Convert.ToUInt64(steamStr);
            return steamId > 0 ? steamId : null;
        }

        private bool IsMultiplayerFromPath(string file)
        {
            var name = file.Split("\\savegame")[1];
            return name.Contains("MP Replay");
        }
        
        public Aoe2Record Read(string path)
        {
            var record = new Aoe2Record();
            record.Started = StartedFromFile(path);
            byte[] fileBytes = ReadAllBytes(path);
            var memory = new MemoryStream(fileBytes);
            var reader = new BinaryReader(memory, Encoding.ASCII);
            record.Read(reader);
            return record;
        }

        private byte[] ReadAllBytes(string path)
        {
            byte[] bytes = null;
            using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                int length = Convert.ToInt32(fs.Length);
                bytes = new byte[(length)];
                fs.Read(bytes, 0, length);
            }
            return bytes;
        }

        private uint StartedFromFile(string path)
        {
            var t1970 = new DateTime(1970, 1, 1);
            // +/- seconds diff to aoe2.net
            return (uint)File.GetCreationTime(path).ToUniversalTime().Subtract(t1970).TotalSeconds;
        }
    }
}