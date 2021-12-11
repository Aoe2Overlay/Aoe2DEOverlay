using System;
using System.IO;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

// 
var scriptsPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(1));

var managerBuildProcess = new Process
{
    StartInfo =
    {
        FileName = " dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "build-manager.csx"
    }
};
managerBuildProcess.Start();
managerBuildProcess.WaitForExit();
var overlayBuildProcess = new Process
{
    StartInfo =
    {
        FileName = " dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "build-overlay.csx"
    }
};
overlayBuildProcess.Start();
overlayBuildProcess.WaitForExit();
