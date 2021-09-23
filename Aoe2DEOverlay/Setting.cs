using System.Diagnostics;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    class Setting
    {
        private static string profileIdKey = "profileId";
        private static string marginTopKey = "bottom";
        private static string marginLeftKey = "right";
        private static string marginRightKey = "left";
        private static string marginBottomKey = "top";
        private static string verticalKey = "vertical";
        private static string horizontalKey = "horizontal";
        private static string fontSizeKey = "fontSize";
        private static string format1v1Key = "format1v1";
        private static string formatTeamKey = "formatTeam";
        
        private static string format1v1Default = "P{slot} {name} <{country}> [#{1v1.rank} E:{1v1.elo} W:{1v1.rate} S:{1v1.streak} G:{1v1.games}]";
        private static string formatTeamDefault = "P{slot} {name} <{country}> [#{1v1.rank} E:{1v1.elo} W:{1v1.rate} S:{1v1.streak} G:{1v1.games}] (#{team.rank} E:{team.elo} W:{team.rate} S:{team.streak} G:{team.games})";
        
        private JObject json = new JObject();
        private string basePath = "";
        private string filePath = "";
        private string fileName = "setting.json";
        private FileSystemWatcher watcher = new FileSystemWatcher(); 
        
        public ISettingObserver Observer;
        public static Setting Instance { get; } = new Setting();
        
        public int ProfileId { get {
            var token = json[profileIdKey];
            if (token == null || token.Value<string>() == null) return -1;
            return token.Value<int>();
        } }
        
        public double MarginTop { get {
            var token = json[marginTopKey];
            if (token == null || token.Value<string>() == null) return 0;
            var top = token.Value<double>();
            return top;
        } }
        
        public double MarginLeft { get {
            var token = json[marginLeftKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginRight { get {
            var token = json[marginRightKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginBottom { get {
            var token = json[marginBottomKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double FontSize { get {
            var token = json[fontSizeKey];
            if (token == null || token.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public string Format1v1 { get {
            var token = json[format1v1Key];
            if (token == null || token.Value<string>() == null) return "";
            return token.Value<string>();
        } }
        
        public string FormatTeam { get {
            var token = json[formatTeamKey];
            if (token == null || token.Value<string>() == null) return "";
            return token.Value<string>();
        } }
        
        public HorizontalAlignment Horizontal { get {
            var token = json[horizontalKey];
            if (token == null || token.Value<string>() == null) return HorizontalAlignment.Center;
            var horizontal = token.Value<string>();
            if (horizontal.ToLower() == "left") return HorizontalAlignment.Left;
            if (horizontal.ToLower() == "right") return HorizontalAlignment.Right;
            return HorizontalAlignment.Center;
        } }
        
        public VerticalAlignment Vertical { get {
            var token = json[verticalKey];
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
            json[profileIdKey] = null;
            json[marginTopKey] = 0;
            json[marginLeftKey] = 0;
            json[marginRightKey] = 0;
            json[marginBottomKey] = 0;
            json[fontSizeKey] = 12;
            json[horizontalKey] = "center";
            json[verticalKey] = "top";
            json[format1v1Key] = format1v1Default;
            json[formatTeamKey] = formatTeamDefault;
            
            basePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            filePath += $"{basePath}/{fileName}";
            if (!System.IO.File.Exists(filePath))
            {
                Save();
            }
            Load();
            watcher.Path = basePath;
            watcher.NotifyFilter = NotifyFilters.Attributes |  
                                   NotifyFilters.CreationTime |  
                                   NotifyFilters.DirectoryName |  
                                   NotifyFilters.FileName |  
                                   NotifyFilters.LastAccess |  
                                   NotifyFilters.LastWrite |  
                                   NotifyFilters.Security | 
                                   NotifyFilters.Size;
            watcher.Changed += OnChanged;
            watcher.Filter = fileName;  
            watcher.EnableRaisingEvents = true; 
        }

        public void Save()
        {
            System.IO.File.WriteAllText(filePath, json.ToString(Formatting.Indented));
        }
        
        public void Load()
        {
            try
            {
                var text = System.IO.File.ReadAllText(filePath);
                json = JObject.Parse(text);
            }
            catch
            {
                // ignored
            }
        }
        
        public void OnChanged(object source, FileSystemEventArgs e)  
        {  
            Load();
            Observer?.Changed();
        }  
        

    }
}
