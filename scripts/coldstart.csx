using System.Threading;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

// 

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));
var publicPath = $"{rootPath}\\publish\\Aoe2DEOverlay";

var x64Path = $"{publicPath}\\x64";
var x86Path = $"{publicPath}\\x86";
var x64Exe = $"{x64Path}\\Aoe2DEOverlay.exe";
var x86Exe = $"{x86Path}\\Aoe2DEOverlay.exe";

var x64Json = $"{x64Path}\\setting.json";
var x86Json = $"{x86Path}\\setting.json";

// * * * * * * * * * * * * * * * * * * 
// x64.exe
// * * * * * * * * * * * * * * * * * * 
var x64Process = new Process
{
    StartInfo =
    {
        FileName = x64Exe,
        CreateNoWindow = true,
        UseShellExecute = false
    }
};
x64Process.Start();
while (!File.Exists(x64Json)) {
    Thread.Sleep(1000);
};
x64Process.Kill();


// * * * * * * * * * * * * * * * * * * 
// x86.exe
// * * * * * * * * * * * * * * * * * * 
var x86Process = new Process
{
    StartInfo =
    {
        FileName = x86Exe,
        CreateNoWindow = true,
        UseShellExecute = false
    }
};
x86Process.Start();
while (!File.Exists(x86Json)) {
    Thread.Sleep(1000);
};
x86Process.Kill();
