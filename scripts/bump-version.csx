#load "util/version.csx"

using System;
using System.IO;
using System.Text.RegularExpressions;

var args =  Args.ToList();

if(args.Count < 1) {
    Console.WriteLine("ERROR: no version as argument found");
    return;
}
var version = Args.ToList().First<string>() ?? "";

if(!Version.IsValid(version)) {
    Console.WriteLine("ERROR: invalid version");
    return;
}

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));
var metaPath = $"{rootPath}\\Aoe2DEOverlay\\AppInfo\\Metadata.cs";
var projPath = $"{rootPath}\\Aoe2DEOverlay\\Aoe2DEOverlay.csproj";

string metadataText = File.ReadAllText(metaPath);
string projectText = File.ReadAllText(projPath);

Regex metadataRegex = new Regex("Version Version.*=.*new.*Version\\(\".*\"\\)");
Regex projectVersionRegex = new Regex("<Version>.*<\\/Version>");
Regex projectPackageRegex = new Regex("<PackageVersion>.*<\\/PackageVersion>");


metadataText = metadataRegex.Replace(metadataText, $"Version Version = new Version(\"{version}\")");
projectText = projectVersionRegex.Replace(projectText, $"<Version>{version}</Version>");
projectText = projectPackageRegex.Replace(projectText, $"<PackageVersion>{version}</PackageVersion>");

Encoding encoding = new UTF8Encoding(true);
File.WriteAllText(metaPath, metadataText, encoding);
File.WriteAllText(projPath, projectText, encoding);
 