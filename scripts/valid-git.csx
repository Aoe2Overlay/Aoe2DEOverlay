using System;
using System.IO;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

// 
var scriptsPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(1));

var gitCurrentProcess = new Process
{
    StartInfo =
    {
        FileName = "git",
        WorkingDirectory = scriptsPath,
        Arguments = "branch --show-current",
        RedirectStandardOutput = true
    }
};
gitCurrentProcess.Start();
gitCurrentProcess.WaitForExit();
var currentBranch = gitCurrentProcess.StandardOutput.ReadToEnd().Trim();

Console.WriteLine($"git branch main: {currentBranch == "main"}");

var gitStatusProcess = new Process
{
    StartInfo =
    {
        FileName = "git",
        WorkingDirectory = scriptsPath,
        //Arguments = "-C \"C:\\Users\\Jan\\Desktop\\temp-git-test\" status -s",
        Arguments = " status -s",
        RedirectStandardOutput = true
    }
};
gitStatusProcess.Start();
gitStatusProcess.WaitForExit();
var isStatusOk = gitStatusProcess.StandardOutput.ReadToEnd() == "";

Console.WriteLine($"git status ok: {isStatusOk}");




