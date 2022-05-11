using System;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Timer = System.Timers.Timer;

namespace Aoe2DEOverlay
{
    /*
     * TODO STATE
     * AppState
     * - check update
     * - updating
     * - installing (unpacking) & replace exe
     * - error
     * - ready
     *
     * if AppState.ready then:
     * 
     *     Aoe2NetPlayerState
     *     - loading
     *     - error
     *     - ready
     * 
     *     Aoe2NetMatchState
     *     - loading
     *     - error
     *     - ready
     *
     *     Aoe2RecordState
     *     - reading
     *     - error
     *     - ready
     * 
     */
    public partial class MainWindow : Window
    {
        private Timer updateAvailableTimer;
        public MainWindow()
        {
            if (Metadata.HasSecret)
            {
                AppCenter.Start(Metadata.Secret.AppCenterKey, typeof(Analytics), typeof(Crashes));
            }
            InitializeComponent();
            Setting.Instance.OnSettingChange += _ => SettingChanged();
            WatchRecordService.Instance.OnMatchUpdate += match => UpdateMatch(match);
            PlayerStatsService.Instance.OnPlayerUpdate += match =>  UpdateMatch(match);
            MatchStateService.Instance.OnServerUpdate += match =>  UpdateMatch(match);
            CheckUpdateService.Instance.OnNewVersion += (version, url) => UpdateAvailable(version);
            DownloadUpdateService.Initialize();
            InstallUpdateService.Initialize();
            CheckUpdateService.Instance.CheckRelease();
            
            StateView.Visibility = Visibility.Hidden;
            CheckUpdateService.Instance.OnNoUpdates += () =>
            {
                StateView.Visibility = Visibility.Visible;
                LoadingState();
                ApplySettings();
            };
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwnd = new WindowInteropHelper(this).Handle;
            WindowHelper.SetWindowExTransparent(hwnd);
        }
        
        public void LoadingState()
        {
            UpdatePanel.Visibility = Visibility.Collapsed;
            ServerPanel.Visibility = Visibility.Collapsed; 
        }

        public void UpdateMatch(Match match)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action<Match>(UpdateMatch), match);
                return;
            }
            ServerPanel.Visibility = Visibility.Visible;
            
            ServerLabel.Content = ServerLabelText(match);
        }

        private string ServerLabelText(Match match)
        {
            var text =  Setting.Instance.Server.Format;
            text = text.Replace("{record}", $"{(match.IsRecordRead ? "" : "(!)")}");
            text = text.Replace("{server.key}", $"{match.ServerKey}");
            text = text.Replace("{server.name}", $"{match.ServerName}");
            text = text.Replace("{mode.name}", $"{match.GameTypeName}");
            text = text.Replace("{mode.short}", $"{match.GameTypeShort}");
            text = text.Replace("{ranked}", $"{(match.IsRanked ? "Ranked" : "Unranked")}");
            text = text.Replace("{map.name}", $"{match.MapName}");
            return text;
        }

        public void SettingChanged()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(SettingChanged));
                return;
            }

            ApplySettings();
        }
        public void ApplySettings()
        {
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