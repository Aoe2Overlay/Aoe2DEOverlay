using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using ReadAoe2Recrod;

namespace Aoe2DEOverlay
{
    public delegate void WatchRecordUpdate(Match match);
    public class WatchRecordService
    {
        private string homePath = Environment.GetEnvironmentVariable("HOMEPATH");
        private string basePath;
        private string filter = "*.aoe2record";
        private string lastFile = "";

        private FileSystemWatcher watcher = new();

        public WatchRecordUpdate Subscriber;
        
        public static WatchRecordService Instance { get; } = new ();

        WatchRecordService()
        {
            StartSaveGameWatching();
        }

        public void StartSaveGameWatching()
        {
            basePath = $"{homePath}\\Games\\Age of Empires 2 DE\\76561197961425297\\savegame";
            watcher.Path = basePath;
            watcher.NotifyFilter = NotifyFilters.Attributes |  
                                   NotifyFilters.CreationTime |  
                                   NotifyFilters.DirectoryName |  
                                   NotifyFilters.FileName |  
                                   NotifyFilters.LastAccess |  
                                   NotifyFilters.LastWrite |  
                                   NotifyFilters.Security | 
                                   NotifyFilters.Size;
            watcher.Changed += OnFileChanged;
            watcher.Filter = filter;  
            watcher.EnableRaisingEvents = true; 
            
            var file =  new DirectoryInfo(basePath)
                .GetFiles()
                .OrderByDescending(f => f.LastWriteTime)
                .First().FullName;
            
            var t1970 = new DateTime(1970, 1, 1);
            var before = (uint)File.GetLastWriteTime(file).Subtract(t1970).TotalSeconds;
            var timer = new Timer(5000);
            timer.AutoReset = false;
            timer.Elapsed += (sender, args) =>
            {
                var after = (uint)File.GetLastWriteTime(file).Subtract(t1970).TotalSeconds;
                if(before < after) OnChanged(file);
            };
            timer.Start();
        }
        
        public void OnFileChanged(object source, FileSystemEventArgs e)  
        {  
            var file = new FileInfo(e.FullPath).FullName;
            OnChanged(file);
        } 
        
        public void OnChanged(string file)  
        {  
            if (file == lastFile) return; // ignore
            lastFile = file;
            try
            {
                var record = Read(file);
                var match = MatchFromRecord(record);
                Subscriber(match);
                var message = new WatchRecordMessage(match);
                MessageBus.Instance.Subscriber(message);
            }
            catch (Exception exception)
            {
                lastFile = "";
            }
        } 
        
        private Match MatchFromRecord(Aoe2Record record)
        {
            var match = new Match();
            match.Started = record.Started;
            match.Difficulty = record.Difficulty;
            match.IsMultiplayer = record.IsMultiplayer;
            match.IsRanked = record.IsRanked;
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
        
        public Aoe2Record Read(string path)
        {
            var t1970 = new DateTime(1970, 1, 1);
            var record = new Aoe2Record();
            record.Started =  (uint)File.GetCreationTime(path).Subtract(t1970).TotalSeconds; // +/- 5 seconds diff to aoe2.net
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
    }
}