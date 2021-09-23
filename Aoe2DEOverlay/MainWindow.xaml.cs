using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Aoe2DEOverlay
{
    public partial class MainWindow : Window, IServiceObserver, ISettingObserver
    {
        public MainWindow()
        {
            InitializeComponent();
            var settings = Setting.Instance;
            Service.Instance.ProfileId = Setting.Instance.ProfileId;
            Service.Instance.observer = this;
            Service.Instance.Start();
            Setting.Instance.Observer = this;
            LoadingState();
            Changed();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowHelper.SetWindowExTransparent(hwnd);
        }
        
        public void LoadingState()
        {
            LoadingLabel.Visibility = Visibility.Visible; 
            LoadingLabel.Content = "Loading…";
            
            P1Label.Visibility = Visibility.Collapsed; 
            P2Label.Visibility = Visibility.Collapsed; 
            P3Label.Visibility = Visibility.Collapsed; 
            P4Label.Visibility = Visibility.Collapsed;
            P5Label.Visibility = Visibility.Collapsed; 
            P6Label.Visibility = Visibility.Collapsed; 
            P7Label.Visibility = Visibility.Collapsed; 
            P8Label.Visibility = Visibility.Collapsed; 
        }

        public void Update(Data data)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<Data>(Update), data);
                return;
            }
            
            LoadingLabel.Visibility = Visibility.Collapsed;
            
            UpdateLabels(data);
        }

        public void UpdateLabels(Data data)
        {
            P1Label.Content = PlayerLabelText(data, 1);
            P2Label.Content = PlayerLabelText(data, 2);
            P3Label.Content = PlayerLabelText(data, 3);
            P4Label.Content = PlayerLabelText(data, 4);
            P5Label.Content = PlayerLabelText(data, 5);
            P6Label.Content = PlayerLabelText(data, 6);
            P7Label.Content = PlayerLabelText(data, 7);
            P8Label.Content = PlayerLabelText(data, 8);

            P1Label.Foreground = PlayerFontColor(data, 1);
            P2Label.Foreground = PlayerFontColor(data, 2);
            P3Label.Foreground = PlayerFontColor(data, 3);
            P4Label.Foreground = PlayerFontColor(data, 4);
            P5Label.Foreground = PlayerFontColor(data, 5);
            P6Label.Foreground = PlayerFontColor(data, 6);
            P7Label.Foreground = PlayerFontColor(data, 7);
            P8Label.Foreground = PlayerFontColor(data, 8);

            P1Label.Visibility = PlayerLabelVisible(data, 1);
            P2Label.Visibility = PlayerLabelVisible(data, 2);
            P3Label.Visibility = PlayerLabelVisible(data, 3);
            P4Label.Visibility = PlayerLabelVisible(data, 4);
            P5Label.Visibility = PlayerLabelVisible(data, 5);
            P6Label.Visibility = PlayerLabelVisible(data, 6);
            P7Label.Visibility = PlayerLabelVisible(data, 7);
            P8Label.Visibility = PlayerLabelVisible(data, 8);
        }

        private string PlayerLabelText(Data data, int slot)
        {
            if (data.players.Count < slot) return "";
            
            var player = data.players[slot  - 1];
            var m1v1 = data.LeaderboardId > 10 ? player.EW1v1 : player.RM1v1;
            var mTeam = data.LeaderboardId > 10 ? player.EWTeam : player.RMTeam;
            var text =  data.players.Count <= 2 ? Setting.Instance.Format1v1 : Setting.Instance.FormatTeam;

            var streak1v1Prefix = m1v1.Streak > 0 ? "+" : mTeam.Streak == 0 ? " " : "";
            var streakteamPrefix = mTeam.Streak > 0 ? "+" : mTeam.Streak == 0 ? " " : "";

            text = text.Replace("{slot}", $"{player.Slot}");
            text = text.Replace("{name}", $"{player.Name}");
            text = text.Replace("{country}", $"{player.Country}");
            
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
            text = text.Replace("{team.streak}", $"{streakteamPrefix}{mTeam.Streak.ToString()}");
            text = text.Replace("{team.games}", $"{mTeam.Games}");
            text = text.Replace("{mTeam.wins}", $"{mTeam.Wins}");
            text = text.Replace("{mTeam.losses}", $"{mTeam.Losses}");
            
            return text;
        }
        
        private Brush PlayerFontColor(Data data, int slot)
        {
            if (data.players.Count < slot)  return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            var p = data.players[slot  - 1];
            if (p.Color == 1) return new SolidColorBrush(Color.FromArgb(255, 60, 120, 255));
            if (p.Color == 2) return new SolidColorBrush(Color.FromArgb(255, 255, 20, 20));
            if (p.Color == 3) return new SolidColorBrush(Color.FromArgb(255, 20, 255, 20));
            if (p.Color == 4) return new SolidColorBrush(Color.FromArgb(255, 250, 250, 10));
            if (p.Color == 5) return new SolidColorBrush(Color.FromArgb(255, 40, 220, 195));
            if (p.Color == 6) return new SolidColorBrush(Color.FromArgb(255, 230, 110, 230));
            if (p.Color == 7) return new SolidColorBrush(Color.FromArgb(255, 210, 210, 210));
            if (p.Color == 8) return new SolidColorBrush(Color.FromArgb(255, 240, 170, 50));
            return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)); 
        }

        private void LabelFontSize(double size)
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
        
        private Visibility PlayerLabelVisible(Data data, int slot) 
        {
            if (data.players.Count < slot) return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public void Changed()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(Changed));
                return;
            }
            
            if(Service.Instance.ProfileId != Setting.Instance.ProfileId) 
            {
                LoadingState();
                Service.Instance.ProfileId = Setting.Instance.ProfileId;
            }
            else if(Service.Instance.Data != null)
            {
                 UpdateLabels(Service.Instance.Data);
            }

            RaitingPanel.Margin = new Thickness(Setting.Instance.MarginLeft, Setting.Instance.MarginTop, Setting.Instance.MarginRight, Setting.Instance.MarginBottom);
            RaitingPanel.HorizontalAlignment = Setting.Instance.Horizontal;
            RaitingPanel.VerticalAlignment = Setting.Instance.Vertical;
            LabelFontSize(Setting.Instance.FontSize);
        }
    }
}