using System;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    public class UpdateSetting
    {
        private JObject json;
        
        private static string marginTopKey = "bottom";
        private static string marginLeftKey = "right";
        private static string marginRightKey = "left";
        private static string marginBottomKey = "top";
        private static string verticalKey = "vertical";
        private static string horizontalKey = "horizontal";
        private static string fontSizeKey = "fontSize";
        private static string prereleaseKey = "prerelease";
        private static string waitingKey = "waiting";
        private static string durationKey = "duration";
        
        private static double fontSizeDefault = 18.0;

        public UpdateSetting(JObject json)
        {
            json[marginTopKey] = 0;
            json[marginLeftKey] = 0;
            json[marginRightKey] = 0;
            json[marginBottomKey] = 0;
            json[fontSizeKey] = fontSizeDefault;
            json[horizontalKey] = "center";
            json[verticalKey] = "center";
            
            this.json = json;
        }

        public void Load(JObject json)
        {
            this.json = json;
        }
        
        public double MarginTop { get {
            var token = json[marginTopKey];
            if (token == null) return 0;
            var top = token.Value<double>();
            return top;
        } }
        
        public double MarginLeft { get {
            var token = json[marginLeftKey];
            if (token == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginRight { get {
            var token = json[marginRightKey];
            if (token == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginBottom { get {
            var token = json[marginBottomKey];
            if (token == null) return 0;
            return token.Value<double>();
        } }
        
        public double FontSize { get {
            var token = json[fontSizeKey];
            if (token == null) return fontSizeDefault;
            return token.Value<double>();
        } }

        public HorizontalAlignment Horizontal { get {
            var token = json[horizontalKey];
            if (token?.Value<string>() == null) return HorizontalAlignment.Center;
            var horizontal = token.Value<string>();
            if (horizontal?.ToLower() == "left") return HorizontalAlignment.Left;
            if (horizontal?.ToLower() == "right") return HorizontalAlignment.Right;
            return HorizontalAlignment.Center;
        } }
        
        public VerticalAlignment Vertical { get {
            var token = json[verticalKey];
            if (token?.Value<string>() == null) return VerticalAlignment.Bottom;
            var vertical = token.Value<string>();
            if (vertical?.ToLower() == "top") return VerticalAlignment.Top;
            if (vertical?.ToLower() == "bottom") return VerticalAlignment.Bottom;
            return VerticalAlignment.Center;
        } }
        
        public bool Prerelease { get {
            var token = json[prereleaseKey];
            if (token == null) return false;
            return token.Value<bool>();
        } }
        
        public bool Waiting { get {
            var token = json[waitingKey];
            if (token == null) return true;
            return token.Value<bool>();
        } }
        
        public int Duration { get {
            var token = json[durationKey];
            if (token == null) return 7000;
            return Math.Max(1000, Math.Min(10000, token.Value<int>() * 1000));
        } }
    }
    
    
}