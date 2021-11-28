using System.Windows;
using System.Windows.Controls;

namespace Aoe2DEOverlay.View
{
    public partial class SplashView : UserControl
    {
        
        public SplashView()
        {
            InitializeComponent();
            UpdateVersion();
            SetupStateControl();
        }

        private void SetupStateControl()
        {
            CheckForUpdateState();
            CheckUpdateService.Instance.OnNewVersion += (version, url) =>
            {
                NewVersionFoundState(version.ToString());
            };
            CheckUpdateService.Instance.OnNoUpdates += () =>
            {
                NoUpdateFoundState();
            };
            DownloadUpdateService.Instance.OnDownloadProgress += (percentage, completed) =>
            {
                DownloadUpdateState(percentage);
                if (completed) InstallUpdateState();
            };
            
            // TODO disappear splashscreen if every thing is up to date
        }

        private void CheckForUpdateState()
        {
            InfoLabel.Content = "Check for update";
        }

        private void NewVersionFoundState(string version)
        {
            InfoLabel.Content = $"Version {version} found";
        }

        private void DownloadUpdateState(int percentage)
        {
            InfoLabel.Content = $"Download update {percentage}%";
        }

        private void NoUpdateFoundState()
        {
            InfoLabel.Content = "Is up to date!";
        }

        private void InstallUpdateState()
        {
            InfoLabel.Content = "Install update!";
        }

        public void UpdateVersion()
        {
            VersionLabel.Content = Metadata.Version.ToString();
        }
        
    }
}