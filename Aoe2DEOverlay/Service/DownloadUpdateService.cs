using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Aoe2DEOverlay
{
    public delegate void OnDownloadProgress(int percentage, bool completed);
    public class DownloadUpdateService
    {
        public static DownloadUpdateService Instance { get; } = new ();

        public static void Initialize()
        {
            var _ = Instance;
        }

        private WebClient client = new();

        public OnDownloadProgress OnDownloadProgress;
        
        static DownloadUpdateService()
        {
        }

        private DownloadUpdateService()
        {
            var basePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var dirPath = $"{basePath}/update";
            if (Directory.Exists(dirPath)) Directory.Delete(dirPath, true);
            
            client.DownloadProgressChanged += (sender, args) =>
            {
                OnDownloadProgress(args.ProgressPercentage, false);
            };
            client.DownloadFileCompleted += (sender, args) =>
            {
                OnDownloadProgress(100, true);
            };
            
            CheckUpdateService.Instance.OnNewVersion += (version, url) => 
            {
                var platform = Metadata.platform.ToString();
                var fileName = $"{platform}.zip";
                DownloadRelease(url, fileName);
            };
        }

        private void DownloadRelease(string url, string fileName)
        {
            var basePath = System.IO.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            var dirPath = $"{basePath}/update";
            var filePath = $"{dirPath}/{fileName}";
            if (Directory.Exists(dirPath)) Directory.Delete(dirPath, true);
            Directory.CreateDirectory(dirPath);
            client.DownloadFileAsync(new Uri(url), filePath);
        }
    }
}