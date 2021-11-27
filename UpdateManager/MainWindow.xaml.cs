using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace UpdateManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            while (IsAoe2OverlayRunning())
            {
                System.Threading.Thread.Sleep(1000);
            }
            CopyUpdate();
            //StartAoe2Overlay();
        }

        private void CopyUpdate()
        {
            var basePath = GetBasePath();
            var platform = IntPtr.Size == 4 ? "x86" : "x64";
            var updatePath = $"{basePath}\\update\\{platform}";

            //Debug1Label.Content = updatePath;
            //Debug2Label.Content = basePath;

            CopyFilesRecursively(updatePath, basePath);
        }

        private bool IsAoe2OverlayRunning()
        {
            // TODO check if Aoe2Overlay Application running
            // TODO make sure only one Instance Application/Window can run (Aoe2DEOverlay & UpdateManager)
            return false;
        }

        private void StartAoe2Overlay()
        {
            var basePath = GetBasePath();
            var exePath = $"{basePath}/Aoe2DEOverlay.exe";
            Process.Start(exePath);
            Application.Current.Shutdown();
        }

        private string GetBasePath()
        {
            return String.Join('\\', System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)
                .Split('\\').SkipLast(2).ToArray());
        }
        
        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}