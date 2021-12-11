using System;
using System.IO;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));
var projectPath = $"{rootPath}\\UpdateManager\\UpdateManager.csproj";

var publishx64Path = $"{rootPath}\\publish\\UpdateManager\\x64";
var argumentsX64 =  $"publish {projectPath} -r win-x64 -o {publishx64Path} -c Release -f net5.0-windows -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --self-contained";

var publishx86Path = $"{rootPath}\\publish\\UpdateManager\\x86";
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

public static void DirectoryCopy(DirectoryInfo source, DirectoryInfo target) {
    foreach (DirectoryInfo dir in source.GetDirectories())
        DirectoryCopy(dir, target.CreateSubdirectory(dir.Name));
    foreach (FileInfo file in source.GetFiles())
        file.CopyTo(Path.Combine(target.FullName, file.Name), true);
}

var outputx64Path = $"{rootPath}\\Aoe2DEOverlay\\UpdateManager\\x64";
var outputx86Path = $"{rootPath}\\Aoe2DEOverlay\\UpdateManager\\x86";

if (Directory.Exists(outputx64Path)) Directory.Delete(outputx64Path, true);
Directory.CreateDirectory(outputx64Path);

if (Directory.Exists(outputx86Path)) Directory.Delete(outputx86Path, true);
Directory.CreateDirectory(outputx86Path);

DirectoryCopy(new DirectoryInfo(publishx64Path), new DirectoryInfo(outputx64Path));
DirectoryCopy(new DirectoryInfo(publishx86Path), new DirectoryInfo(outputx86Path));
