
string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

// 
var scriptsPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(1));

// * * * * * * * * * * * * * * * * * *
// clean.csx
// * * * * * * * * * * * * * * * * * *
var cleanProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "clean.csx"
    }
};
cleanProcess.Start();
cleanProcess.WaitForExit();

// * * * * * * * * * * * * * * * * * *
// valid.csx
// * * * * * * * * * * * * * * * * * *
var validProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "valid.csx",
        RedirectStandardOutput = true
    }
};
validProcess.Start();
validProcess.WaitForExit();
var validOutput = validProcess.StandardOutput.ReadToEnd();

var isValid = validOutput.Contains("Is Valid");
if(!isValid) return;

// * * * * * * * * * * * * * * * * * *
// build.csx
// * * * * * * * * * * * * * * * * * *
var buildProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "build.csx"
    }
};
buildProcess.Start();
buildProcess.WaitForExit();


// * * * * * * * * * * * * * * * * * *
// pack.csx
// * * * * * * * * * * * * * * * * * *

var packProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "pack.csx"
    }
};
packProcess.Start();
packProcess.WaitForExit();

// * * * * * * * * * * * * * * * * * *
// release.csx
// * * * * * * * * * * * * * * * * * *
var releaseProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "release.csx"
    }
};
releaseProcess.Start();
releaseProcess.WaitForExit();
