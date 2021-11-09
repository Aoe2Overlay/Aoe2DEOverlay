using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace Aoe2DEOverlay
{
    // App Center Analytics/Crashlytics 
    // https://docs.microsoft.com/en-us/appcenter/sdk/getting-started/wpf-winforms
    // 
    // secret:
    // https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows
    public partial class MainWindow : Window, ILastMatchObserver, ISettingObserver, IReleaseObserver
    {
        private Timer updateAvailableTimer;
        public MainWindow()
        {
            if (Metadata.HasSecret)
            {
                AppCenter.Start(Metadata.Secret.AppCenterKey, typeof(Analytics), typeof(Crashes));
            }
            InitializeComponent();
            Setting.Instance.Observer = this;
            ReleaseUpdateService.Instance.Observer = this;
            WatchRecordService.Instance.Subscriber += match => UpdateMatch(match);
            PlayerStatsService.Instance.Subscriber += match =>  UpdateMatch(match);
            MatchStateService.Instance.Subscriber += match =>  UpdateMatch(match);
            LoadingState();
            CheckReleases();
            SettingChanged();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowHelper.SetWindowExTransparent(hwnd);
        }

        public void CheckReleases()
        {
            ReleaseUpdateService.Instance.CheckRelease();
        }
        
        public void LoadingState()
        {
            LoadingLabel.Visibility = Visibility.Visible; 
            LoadingLabel.Content = "Loading…";

            UpdatePanel.Visibility = Visibility.Collapsed;
            
            P1Label.Visibility = Visibility.Collapsed; 
            P2Label.Visibility = Visibility.Collapsed; 
            P3Label.Visibility = Visibility.Collapsed; 
            P4Label.Visibility = Visibility.Collapsed;
            P5Label.Visibility = Visibility.Collapsed; 
            P6Label.Visibility = Visibility.Collapsed; 
            P7Label.Visibility = Visibility.Collapsed; 
            P8Label.Visibility = Visibility.Collapsed; 
            
            ServerPanel.Visibility = Visibility.Collapsed; 
        }

        public void UpdateMatch(Match match)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<Match>(UpdateMatch), match);
                return;
            }
            
            LoadingLabel.Visibility = Visibility.Collapsed;
            ServerPanel.Visibility = Visibility.Visible;
            
            ServerLabel.Content = ServerLabelText(match);
            
            UpdateLabels(match);
        }

        public void UpdateLabels(Match match)
        {
            P1Label.Content = PlayerLabelText(match, 1);
            P2Label.Content = PlayerLabelText(match, 2);
            P3Label.Content = PlayerLabelText(match, 3);
            P4Label.Content = PlayerLabelText(match, 4);
            P5Label.Content = PlayerLabelText(match, 5);
            P6Label.Content = PlayerLabelText(match, 6);
            P7Label.Content = PlayerLabelText(match, 7);
            P8Label.Content = PlayerLabelText(match, 8);

            P1Label.Foreground = PlayerFontColor(match, 1);
            P2Label.Foreground = PlayerFontColor(match, 2);
            P3Label.Foreground = PlayerFontColor(match, 3);
            P4Label.Foreground = PlayerFontColor(match, 4);
            P5Label.Foreground = PlayerFontColor(match, 5);
            P6Label.Foreground = PlayerFontColor(match, 6);
            P7Label.Foreground = PlayerFontColor(match, 7);
            P8Label.Foreground = PlayerFontColor(match, 8);

            P1Label.Visibility = PlayerLabelVisible(match, 1);
            P2Label.Visibility = PlayerLabelVisible(match, 2);
            P3Label.Visibility = PlayerLabelVisible(match, 3);
            P4Label.Visibility = PlayerLabelVisible(match, 4);
            P5Label.Visibility = PlayerLabelVisible(match, 5);
            P6Label.Visibility = PlayerLabelVisible(match, 6);
            P7Label.Visibility = PlayerLabelVisible(match, 7);
            P8Label.Visibility = PlayerLabelVisible(match, 8);
        }

        private string ServerLabelText(Match match)
        {
            var text =  Setting.Instance.Server.Format;
            text = text.Replace("{server.key}", $"{match.ServerKey}");
            text = text.Replace("{server.name}", $"{match.ServerName}");
            text = text.Replace("{mode.name}", $"{match.GameTypeName}");
            text = text.Replace("{mode.short}", $"{match.GameTypeShort}");
            text = text.Replace("{ranked}", $"{(match.IsRanked ? "Ranked" : "Unranked")}");
            text = text.Replace("{map.name}", $"{match.MapName}");
            return text;
        }

        private string PlayerLabelText(Match match, int slot)
        {
            if (match.Players.Count < slot) return "";
            
            var player = match.Players[slot  - 1];
            var m1v1 = match.GameTypeShort == "EW" ? player.EW1v1 : player.RM1v1;
            var mTeam = match.GameTypeShort == "EW" ? player.EWTeam : player.RMTeam;
            var text =  match.Players.Count <= 2 ? Setting.Instance.Raiting.Format1v1 : Setting.Instance.Raiting.FormatTeam;

            var streak1v1Prefix = m1v1.Streak > 0 ? "+" : mTeam.Streak == 0 ? " " : "";
            var streakTeamPrefix = mTeam.Streak > 0 ? "+" : mTeam.Streak == 0 ? " " : "";

            text = text.Replace("{slot}", $"{player.Slot}");
            text = text.Replace("{name}", $"{player.Name}");
            text = text.Replace("{country}", $"{(player.Country.Length > 0 ? player.Country : "??")}");
            text = text.Replace("{civ}", $"{player.Civ}");
            text = text.Replace("{id}", $"{player.Id}");
            
            text = text.Replace("{1v1.rank}", $"{m1v1.Rank}");
            text = text.Replace("{1v1.elo}", $"{m1v1.Elo}");
            text = text.Replace("{1v1.rate}", $"{m1v1.WinRate}%");
            text = text.Replace("{1v1.streak}", $"{streak1v1Prefix}{m1v1.Streak.ToString()}");
            text = text.Replace("{1v1.games}", $"{m1v1.Games}");
            text = text.Replace("{1v1.wins}", $"{m1v1.Wins}");
            text = text.Replace("{1v1.losses}", $"{m1v1.Losses}");
            
            text = text.Replace("{team.rank}", $"{mTeam.Rank}");
            text = text.Replace("{team.elo}", $"{mTeam.Elo}");
            text = text.Replace("{team.rate}", $"{mTeam.WinRate}%");
            text = text.Replace("{team.streak}", $"{streakTeamPrefix}{mTeam.Streak.ToString()}");
            text = text.Replace("{team.games}", $"{mTeam.Games}");
            text = text.Replace("{mTeam.wins}", $"{mTeam.Wins}");
            text = text.Replace("{mTeam.losses}", $"{mTeam.Losses}");
            
            return text;
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

        private void RaitingLabelFontSize(double size)
        {
            LoadingLabel.FontSize = size;
            P1Label.FontSize = size;
            P2Label.FontSize = size;
            P3Label.FontSize = size;
            P4Label.FontSize = size;
            P5Label.FontSize = size;
            P6Label.FontSize = size;
            P7Label.FontSize = size;
            P8Label.FontSize = size;
        }
        
        private Visibility PlayerLabelVisible(Match match, int slot) 
        {
            if (match.Players.Count < slot) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public void SettingChanged()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(SettingChanged));
                return;
            }

            RaitingPanel.Margin = new Thickness(Setting.Instance.Raiting.MarginLeft, Setting.Instance.Raiting.MarginTop, Setting.Instance.Raiting.MarginRight, Setting.Instance.Raiting.MarginBottom);
            RaitingPanel.HorizontalAlignment = Setting.Instance.Raiting.Horizontal;
            RaitingPanel.VerticalAlignment = Setting.Instance.Raiting.Vertical;
            RaitingBorder.Background = new SolidColorBrush(Setting.Instance.Raiting.BackgroundColor);
            RaitingBorder.BorderBrush = new SolidColorBrush(Setting.Instance.Raiting.BorderColor);
            RaitingLabelFontSize(Setting.Instance.Raiting.FontSize);

            ServerPanel.Margin = new Thickness(Setting.Instance.Server.MarginLeft, Setting.Instance.Server.MarginTop, Setting.Instance.Server.MarginRight, Setting.Instance.Server.MarginBottom);
            ServerPanel.HorizontalAlignment = Setting.Instance.Server.Horizontal;
            ServerPanel.VerticalAlignment = Setting.Instance.Server.Vertical;
            ServerLabel.FontSize = Setting.Instance.Server.FontSize;
            
            UpdatePanel.Margin = new Thickness(Setting.Instance.Update.MarginLeft, Setting.Instance.Update.MarginTop, Setting.Instance.Update.MarginRight, Setting.Instance.Update.MarginBottom);
            UpdatePanel.HorizontalAlignment = Setting.Instance.Update.Horizontal;
            UpdatePanel.VerticalAlignment = Setting.Instance.Update.Vertical;
            UpdateLabel.FontSize = Setting.Instance.Update.FontSize;
        }

        public void UpdateAvailable(Version version)
        {
            UpdatePanel.Visibility = Visibility.Visible;
            UpdateLabel.Content = $"New version available ({version.ToString()})";
            
            updateAvailableTimer = new Timer(Setting.Instance.Update.Duration);
            updateAvailableTimer.Elapsed += UpdateAvailableTimerTick;
            updateAvailableTimer.Enabled = true;
        }

        private void UpdateAvailableTimerTick(object sender, ElapsedEventArgs e)
        {
            HideUpdatePanel();
            updateAvailableTimer = null;
        }

        private void HideUpdatePanel()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(HideUpdatePanel));
                return;
            }
            UpdatePanel.Visibility = Visibility.Collapsed;
        }
    }
}