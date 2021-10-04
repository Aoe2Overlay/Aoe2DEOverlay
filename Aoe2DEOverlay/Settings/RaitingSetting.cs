using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace Aoe2DEOverlay
{
    class RaitingSetting
    {
        private JObject json;
        
        private static string marginTopKey = "bottom";
        private static string marginLeftKey = "right";
        private static string marginRightKey = "left";
        private static string marginBottomKey = "top";
        private static string verticalKey = "vertical";
        private static string horizontalKey = "horizontal";
        private static string fontSizeKey = "fontSize";
        private static string format1v1Key = "format1v1";
        private static string formatTeamKey = "formatTeam";
        
        private static string borderColorKey = "borderColor";
        private static string backgroundColorKey = "backgroundColor";
        private static string player1ColorKey = "player1Color";
        private static string player2ColorKey = "player2Color";
        private static string player3ColorKey = "player3Color";
        private static string player4ColorKey = "player4Color";
        private static string player5ColorKey = "player5Color";
        private static string player6ColorKey = "player6Color";
        private static string player7ColorKey = "player7Color";
        private static string player8ColorKey = "player8Color";
        
        private static string format1v1Default = "P{slot} {name} <{country}> [#{1v1.rank} E:{1v1.elo} W:{1v1.rate} S:{1v1.streak} G:{1v1.games}]";
        private static string formatTeamDefault = "P{slot} {name} <{country}> [#{1v1.rank} E:{1v1.elo} W:{1v1.rate} S:{1v1.streak} G:{1v1.games}] (#{team.rank} E:{team.elo} W:{team.rate} S:{team.streak} G:{team.games})";
        
        public double MarginTop { get {
            var token = json[marginTopKey];
            if (token?.Value<string>() == null) return 0;
            var top = token.Value<double>();
            return top;
        } }
        
        public double MarginLeft { get {
            var token = json[marginLeftKey];
            if (token?.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginRight { get {
            var token = json[marginRightKey];
            if (token?.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double MarginBottom { get {
            var token = json[marginBottomKey];
            if (token?.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public double FontSize { get {
            var token = json[fontSizeKey];
            if (token?.Value<string>() == null) return 0;
            return token.Value<double>();
        } }
        
        public string Format1v1 { get {
            var token = json[format1v1Key];
            if (token?.Value<string>() == null) return "";
            return token.Value<string>();
        } }
        
        public string FormatTeam { get {
            var token = json[formatTeamKey];
            if (token?.Value<string>() == null) return "";
            return token.Value<string>();
        } }
        
        public Color Player1Color { get {
            var token = json[player1ColorKey];
            var p1color = Color.FromArgb(255, 60, 120, 255);
            if (token?.Value<string>() == null) return p1color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p1color);
            return color;
        } }
        
        public Color Player2Color { get {
            var token = json[player2ColorKey];
            var p2color = Color.FromArgb(255, 255, 20, 20);
            if (token?.Value<string>() == null) return p2color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p2color);
            return color;
        } }
        
        public Color Player3Color { get {
            var token = json[player3ColorKey];
            var p3color = Color.FromArgb(255, 20, 255, 20);
            if (token?.Value<string>() == null) return p3color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p3color);
            return color;
        } }
        
        public Color Player4Color { get {
            var token = json[player4ColorKey];
            var p4color = Color.FromArgb(255, 250, 250, 10);
            if (token?.Value<string>() == null) return p4color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p4color);
            return color;
        } }
        
        public Color Player5Color { get {
            var token = json[player5ColorKey];
            var p5color = Color.FromArgb(255, 40, 220, 195);
            if (token?.Value<string>() == null) return p5color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p5color);
            return color;
        } }
        
        public Color Player6Color { get {
            var token = json[player6ColorKey];
            var p6color = Color.FromArgb(255, 230, 110, 230);
            if (token?.Value<string>() == null) return p6color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p6color);
            return color;
        } }
        
        public Color Player7Color { get {
            var token = json[player7ColorKey];
            var p7color = Color.FromArgb(255, 210, 210, 210);
            if (token?.Value<string>() == null) return p7color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p7color);
            return color;
        } }
        
        public Color Player8Color { get {
            var token = json[player8ColorKey];
            var p8color = Color.FromArgb(255, 240, 170, 50);
            if (token?.Value<string>() == null) return p8color;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? p8color);
            return color;
        } }
        
        public Color BackgroundColor { get {
            var token = json[backgroundColorKey];
            var black = Color.FromArgb(187, 0, 0, 0);
            if (token?.Value<string>() == null) return black;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? black);
            return color;
        } }

        
        public Color BorderColor { get {
            var token = json[borderColorKey];
            var white = Color.FromArgb(128, 255, 255, 255);
            if (token?.Value<string>() == null) return white;
            var color = (Color) (ColorConverter.ConvertFromString(token.Value<string>()) ?? white);
            return color;
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
            if (token?.Value<string>() == null) return VerticalAlignment.Center;
            var vertical = token.Value<string>();
            if (vertical?.ToLower() == "top") return VerticalAlignment.Top;
            if (vertical?.ToLower() == "bottom") return VerticalAlignment.Bottom;
            return VerticalAlignment.Center;
        } }

        public RaitingSetting(JObject json)
        {
            json[marginTopKey] = 0;
            json[marginLeftKey] = 0;
            json[marginRightKey] = 0;
            json[marginBottomKey] = 0;
            json[fontSizeKey] = 12;
            json[horizontalKey] = "center";
            json[verticalKey] = "top";
            json[format1v1Key] = format1v1Default;
            json[formatTeamKey] = formatTeamDefault;

            json[backgroundColorKey] = "#BB000000";
            json[borderColorKey] = "#80FFFFFF";
            json[player1ColorKey] = "#3C78FF";
            json[player2ColorKey] = "#FF1414";
            json[player3ColorKey] = "#14FF14";
            json[player4ColorKey] = "#FAFA0A";
            json[player5ColorKey] = "#28DCC3";
            json[player6ColorKey] = "#E66EE6";
            json[player7ColorKey] = "#D2D2D2";
            json[player8ColorKey] = "#F0AA32";
            
            this.json = json;
        }

        public void Load(JObject json)
        {
            this.json = json;
        }
    }
}