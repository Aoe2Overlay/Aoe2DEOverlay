string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}
var scriptsPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(1));

var isValid = true;


// * * * * * * * * * * * * * * * * * *
// current-version.csx
// * * * * * * * * * * * * * * * * * *
var currentVersionProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "current-version.csx",
        RedirectStandardOutput = true
    }
};
currentVersionProcess.Start();
currentVersionProcess.WaitForExit();
var currentVersionOutput = currentVersionProcess.StandardOutput.ReadToEnd();

isValid = isValid && currentVersionOutput.Contains("OK: Local Version is newer");

// * * * * * * * * * * * * * * * * * *
// valid-git.csx
// * * * * * * * * * * * * * * * * * *
var validGitProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "valid-git.csx",
        RedirectStandardOutput = true
    }
};
validGitProcess.Start();
validGitProcess.WaitForExit();
var validGitOutput = validGitProcess.StandardOutput.ReadToEnd();

isValid = isValid && validGitOutput.Contains("git branch main: True");
isValid = isValid && validGitOutput.Contains("git status ok: True");

// * * * * * * * * * * * * * * * * * *
// valid-secret.csx
// * * * * * * * * * * * * * * * * * *
var validSecretProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet-script",
        WorkingDirectory = scriptsPath,
        Arguments = "valid-secret.csx",
        RedirectStandardOutput = true
    }
};
validSecretProcess.Start();
validSecretProcess.WaitForExit();
var validSecretOutput = validSecretProcess.StandardOutput.ReadToEnd();
isValid = isValid && validSecretOutput.Contains("OK: Secret Exists");

// * * * * * * * * * * * * * * * * * *

Console.WriteLine(isValid ? "Is Valid" : "Is not Valid");
if(!isValid) {
    Console.WriteLine(currentVersionOutput);
    Console.WriteLine(validGitOutput);
    Console.WriteLine(validSecretOutput);
}