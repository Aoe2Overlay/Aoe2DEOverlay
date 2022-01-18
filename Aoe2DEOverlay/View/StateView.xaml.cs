using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Aoe2DEOverlay.View
{
    public partial class StateView : UserControl
    {
        private List<Label> _labels = new ();
        public StateView()
        {
            InitializeComponent();
            ApplySettings(Setting.Instance);
            Setting.Instance.OnSettingChange += setting => ApplySettings(setting);
            WatchRecordService.Instance.OnMatchUpdate += match => UpdateMatch(match);
            PlayerStatsService.Instance.OnPlayerUpdate += match =>  UpdateMatch(match);
            MatchStateService.Instance.OnServerUpdate += match =>  UpdateMatch(match);
        }

        private void ApplySettings(Setting setting)
        {
            // TODO EXTRA MARGIN REMOVE AFTER DEVEOPMENT:
            var devMargin = 0;
            
            Panel.Margin = new Thickness(Setting.Instance.Raiting.MarginLeft, Setting.Instance.Raiting.MarginTop + devMargin, Setting.Instance.Raiting.MarginRight, Setting.Instance.Raiting.MarginBottom);
            Panel.HorizontalAlignment = Setting.Instance.Raiting.Horizontal;
            Panel.VerticalAlignment = Setting.Instance.Raiting.Vertical;
            Border.Background = new SolidColorBrush(Setting.Instance.Raiting.BackgroundColor);
            Border.BorderBrush = new SolidColorBrush(Setting.Instance.Raiting.BorderColor);
            LabelFontSize(Setting.Instance.Raiting.FontSize);
            
        }

        private void LabelFontSize(double size)
        {
            Loading.FontSize = size;
            _labels.ForEach(label => { label.FontSize = size; });
        }

        public void UpdateMatch(Match match)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<Match>(UpdateMatch), match);
                return;
            }

            Loading.Visibility = Visibility.Collapsed;
            UpdateLabels(match);
        }

        public void UpdateLabels(Match match)
        {
            ClearStack();
            var format =  match.Players.Count <= 2 ? Setting.Instance.Raiting.Formats1v1 : Setting.Instance.Raiting.FormatsTeam;
            format.ToList().ForEach(subformat =>
            {
                var leftColumn = CreateColumn();
                var rightColumn = CreateColumn();
                var index = 0;
                match.Players.ForEach(player =>
                {
                    var isLeftSide = index % 2 == 0;
                    var text = PlayerLabelText(match, player.Slot, subformat);
                    var color = PlayerFontColor(match, player.Slot);
                    var alignment = FormatAlignment(subformat, isLeftSide);
                    var label = CreateLabel(text, color, alignment);
                    var column = isLeftSide ? leftColumn : rightColumn;
                    column.Children.Add(label);
                    index += 1;
                });
                LeftStack.Children.Add(leftColumn);
                RightStack.Children.Add(rightColumn);
            });
        }

        public void ClearStack()
        {
            _labels.Clear();
            LeftStack.Children.Clear();
            RightStack.Children.Clear();
        }
        public StackPanel CreateColumn()
        {
            var stack = new StackPanel();
            stack.Margin = new Thickness(0);
            return stack;
        }

        public Label CreateLabel(string content, Brush color, HorizontalAlignment alignment)
        {
            var label = new Label();
            label.Content = content;
            label.Foreground = color;
            label.HorizontalAlignment = alignment;
            // VerticalAlignment="Top"
            label.FontSize = Setting.Instance.Raiting.FontSize;
            label.Margin = new Thickness(0);
            label.Padding = new Thickness(0);
            _labels.Add(label);
            return label;
        }
        
        private Brush PlayerFontColor(Match match, int slot)
        {
            if (match.Players.Count < slot)  return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            var p = match.Players[slot  - 1];
            if (p.Color == 1) return new SolidColorBrush(Setting.Instance.Raiting.Player1Color);
            if (p.Color == 2) return new SolidColorBrush(Setting.Instance.Raiting.Player2Color);
            if (p.Color == 3) return new SolidColorBrush(Setting.Instance.Raiting.Player3Color);
            if (p.Color == 4) return new SolidColorBrush(Setting.Instance.Raiting.Player4Color);
            if (p.Color == 5) return new SolidColorBrush(Setting.Instance.Raiting.Player5Color);
            if (p.Color == 6) return new SolidColorBrush(Setting.Instance.Raiting.Player6Color);
            if (p.Color == 7) return new SolidColorBrush(Setting.Instance.Raiting.Player7Color);
            if (p.Color == 8) return new SolidColorBrush(Setting.Instance.Raiting.Player8Color);
            return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); 
        }

        private string PlayerLabelText(Match match, int slot, string format)
        {
            if (match.Players.Count < slot) return "";
            
            var player = match.Players[slot  - 1];
            var m1v1 = match.GameTypeShort == "EW" ? player.EW1v1 : player.RM1v1;
            var mTeam = match.GameTypeShort == "EW" ? player.EWTeam : player.RMTeam;

            var text = format;
            var streak1v1Prefix = m1v1.Streak > 0 ? "+" : mTeam.Streak == 0 ? " " : "";
            var streakTeamPrefix = mTeam.Streak > 0 ? "+" : mTeam.Streak == 0 ? " " : "";

            text = text.Replace("{slot}", $"{player.Slot}");
            text = text.Replace("{name}", $"{player.Name}");
            text = text.Replace("{country}", $"{(player.Country.Length > 0 ? player.Country : "??")}");
            text = text.Replace("{civ}", $"{player.Civ}");
            text = text.Replace("{id}", $"{player.Id}");
            
            text = text.Replace("{1v1.rank}", $"{m1v1.Rank}");
            text = text.Replace("{1v1.elo}", $"{EloToString(m1v1.Elo)}");
            text = text.Replace("{1v1.rate}", $"{m1v1.WinRate}%");
            text = text.Replace("{1v1.streak}", $"{streak1v1Prefix}{m1v1.Streak.ToString()}");
            text = text.Replace("{1v1.games}", $"{m1v1.Games}");
            text = text.Replace("{1v1.wins}", $"{m1v1.Wins}");
            text = text.Replace("{1v1.losses}", $"{m1v1.Losses}");
            
            text = text.Replace("{team.rank}", $"{mTeam.Rank}");
            text = text.Replace("{team.elo}", $"{EloToString(mTeam.Elo)}");
            text = text.Replace("{team.rate}", $"{mTeam.WinRate}%");
            text = text.Replace("{team.streak}", $"{streakTeamPrefix}{mTeam.Streak.ToString()}");
            text = text.Replace("{team.games}", $"{mTeam.Games}");
            text = text.Replace("{mTeam.wins}", $"{mTeam.Wins}");
            text = text.Replace("{mTeam.losses}", $"{mTeam.Losses}");
            
            return text;
        }

        public string EloToString(int elo)
        {
            if (elo < 10) return $"   {elo}";
            if (elo < 100) return $"  {elo}";
            if (elo < 1000) return $" {elo}";
            return $"{elo}";
        }

        private HorizontalAlignment FormatAlignment(string format, bool isLeftSide)
        {
            var isRightAlignment = format
                is "{1v1.rank}"
                or "{1v1.elo}"
                or "{1v1.rate}"
                or "{1v1.streak}"
                or "{1v1.games}"
                or "{1v1.wins}"
                or "{1v1.losses}"
                or "{team.rank}"
                or "{team.elo}"
                or "{team.rate}"
                or "{team.streak}"
                or "{team.games}"
                or "{mTeam.wins}"
                or "{mTeam.losses}";

            var alignment = isLeftSide ? HorizontalAlignment.Right : HorizontalAlignment.Left;
            if (isRightAlignment) alignment = HorizontalAlignment.Right;
            
            return alignment;
        }
    }
}