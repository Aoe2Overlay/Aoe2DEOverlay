using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    class Setting
    {
        private static string ProfileIdKey = "profileId";
        private static string MarginTopKey = "top";
        private static string MarginLeftKey = "right";
        private static string MarginRightKey = "left";
        private static string MarginBottomKey = "bottom";
        private static string VerticalKey = "vertical";
        private static string HorizontalKey = "horizontal";
        private static string FontSizeKey = "fontSize";
        private static string Format1v1Key = "format1v1";
        private static string FormatTeamKey = "formatTeam";
        
        private static string Format1v1Default = "P{slot} {name} <{country}> [#{1v1.rank} E:{1v1.elo} W:{1v1.rate} S:{1v1.streak} G:{1v1.games}]";
        private static string FormatTeamDefault = "P{slot} {name} <{country}> [#{1v1.rank} E:{1v1.elo} W:{1v1.rate} S:{1v1.streak} G:{1v1.games}] (#{team.rank} E:{team.elo} W:{team.rate} S:{team.streak} G:{team.games})";
        
        private JObject Json = new JObject();
        private string BasePath = "";
        private string FilePath = "";
        private string FileName = "setting.json";
        private FileSystemWatcher watcher = new FileSystemWatcher(); 
        
        public ISettingObserver observer;
        public static Setting Instance { get; } = new Setting();
        
        public int ProfileId { get {
            var token = Json[ProfileIdKey];
            if (token == null || token.Value<string>() == null) return -1;
            return token.Value<int>();
        } }
        
        public double MarginTop { get {
            var token = Json[MarginTopKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginLeft { get {
            var token = Json[MarginLeftKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginRight { get {
            var token = Json[MarginRightKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginBottom { get {
            var token = Json[MarginBottomKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double FontSize { get {
            var token = Json[FontSizeKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public string Format1v1 { get {
            var token = Json[Format1v1Key];
            if (token == null || token.Value<string>() == null) return "";
            return token.Value<string>();
        } }
        
        public string FormatTeam { get {
            var token = Json[FormatTeamKey];
            if (token == null || token.Value<string>() == null) return "";
            return token.Value<string>();
        } }
        
        public HorizontalAlignment Horizontal { get {
            var token = Json[HorizontalKey];
            if (token == null || token.Value<string>() == null) return HorizontalAlignment.Center;
            var horizontal = token.Value<string>();
            if (horizontal.ToLower() == "left") return HorizontalAlignment.Left;
            if (horizontal.ToLower() == "right") return HorizontalAlignment.Right;
            return HorizontalAlignment.Center;
        } }
        
        public VerticalAlignment Vertical { get {
            var token = Json[VerticalKey];
            if (token == null || token.Value<string>() == null) return VerticalAlignment.Center;
            var horizontal = token.Value<string>();
            if (horizontal.ToLower() == "top") return VerticalAlignment.Top;
            if (horizontal.ToLower() == "bottom") return VerticalAlignment.Bottom;
            return VerticalAlignment.Center;
        } }

        static Setting()
        {
        }

        private Setting()
        {
            Json[ProfileIdKey] = null;
            Json[MarginTopKey] = 0;
            Json[MarginLeftKey] = 0;
            Json[MarginRightKey] = 0;
            Json[MarginBottomKey] = 0;
            Json[FontSizeKey] = 12;
            Json[HorizontalKey] = "center";
            Json[VerticalKey] = "top";
            Json[Format1v1Key] = Format1v1Default;
            Json[FormatTeamKey] = FormatTeamDefault;
            
            BasePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            FilePath += $"{BasePath}/{FileName}";
            if (!System.IO.File.Exists(FilePath))
            {
                Save();
            }
            Load();
            watcher.Path = BasePath;
            watcher.NotifyFilter = NotifyFilters.Attributes |  
                                   NotifyFilters.CreationTime |  
                                   NotifyFilters.DirectoryName |  
                                   NotifyFilters.FileName |  
                                   NotifyFilters.LastAccess |  
                                   NotifyFilters.LastWrite |  
                                   NotifyFilters.Security |  
                                   NotifyFilters.Size;
            watcher.Changed += new FileSystemEventHandler(OnChanged);  
            watcher.Filter = FileName;  
            watcher.EnableRaisingEvents = true; 
        }

        public void Save()
        {
            System.IO.File.WriteAllText(FilePath, Json.ToString(Formatting.Indented));
        }
        
        public void Load()
        {
            try
            {
                var text =System.IO.File.ReadAllText(FilePath);
                Json = JObject.Parse(text);
            }
            catch
            {
                // ignored
            }
        }
        
        public void OnChanged(object source, FileSystemEventArgs e)  
        {  
            Load();
            observer?.Changed();
        }  
        

    }
}
