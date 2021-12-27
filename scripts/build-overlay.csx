using System;
using System.IO;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));
var projectPath = $"{rootPath}\\Aoe2DEOverlay\\Aoe2DEOverlay.csproj";

var publishx64Path = $"{rootPath}\\publish\\Aoe2DEOverlay\\x64";
var argumentsX64 =  $"publish {projectPath} -r win-x64 -o {publishx64Path} -c Release -f net5.0-windows -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained";

var publishx86Path = $"{rootPath}\\publish\\Aoe2DEOverlay\\x86";
var argumentsX86 =  $"publish {projectPath} -r win-x86 -o {publishx86Path} -c Release -f net5.0-windows -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained";

var x64BuildProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet",
        WorkingDirectory = rootPath,
        Arguments = argumentsX64
    }
};
x64BuildProcess.Start();
x64BuildProcess.WaitForExit();

var x86BuildProcess = new Process
{
    StartInfo =
    {
        FileName = "dotnet",
        WorkingDirectory = rootPath,
        Arguments = argumentsX86
    }
};
x86BuildProcess.Start();
x86BuildProcess.WaitForExit();