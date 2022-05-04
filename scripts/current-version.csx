#load "util/version.csx"
#r "nuget: Newtonsoft.Json, 13.0.1"

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Text.RegularExpressions;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));


var defaultColor = Console.ForegroundColor;

// * * * * * * * * * * * * * * * * * * * * * * * * * *
// local version
// * * * * * * * * * * * * * * * * * * * * * * * * * *
var metaPath = $"{rootPath}\\Aoe2DEOverlay\\AppInfo\\Metadata.cs";
var projPath = $"{rootPath}\\Aoe2DEOverlay\\Aoe2DEOverlay.csproj";

string metadataText = File.ReadAllText(metaPath);
string projectText = File.ReadAllText(projPath);

string metadataPattern = "Version Version.*=.*new.*Version\\(\".*\"\\)";
string projectVersionPattern = "<Version>.*<\\/Version>";
string projectPackagePattern = "<PackageVersion>.*<\\/PackageVersion>";


var metaVersion = Regex.Match(metadataText, metadataPattern).Value;
var metaRegex = new Regex("Version Version.*=.*new.*Version\\(\"|\"\\)");
metaVersion = metaRegex.Replace(metaVersion, "");



var projVersion = Regex.Match(projectText, projectVersionPattern).Value;
var projRegex = new Regex("<Version>|<\\/Version>");
projVersion = projRegex.Replace(projVersion, "");

var packVersion = Regex.Match(projectText, projectPackagePattern).Value;
var packRegex = new Regex("<PackageVersion>|<\\/PackageVersion>");
packVersion = packRegex.Replace(packVersion, "");


if(metaVersion == projVersion && metaVersion == packVersion)
{
    Console.Write("Local Version:");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($" {metaVersion}");
    Console.ForegroundColor = defaultColor;
}
else 
{
    Console.WriteLine("Local Version: ERROR");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(metaVersion);
    Console.WriteLine(projVersion);
    Console.WriteLine(packVersion);
    Console.ForegroundColor = defaultColor;
    Console.WriteLine("");
}

// * * * * * * * * * * * * * * * * * * * * * * * * * *
// remote version
// * * * * * * * * * * * * * * * * * * * * * * * * * *
var http = new HttpClient();

var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/repos/Aoe2Overlay/Aoe2DEOverlay/releases/latest");
var product = new ProductInfoHeaderValue("ReleaseScript", "1.0");
request.Headers.UserAgent.Add(product);

var response = http.Send(request);
var reader = new StreamReader(response.Content.ReadAsStream());
var body = reader.ReadToEnd();
JObject jbody = JObject.Parse(body);

var latestVersion = jbody["tag_name"].Value<string>();
var latestRegex = new Regex("^v");
latestVersion = latestRegex.Replace(latestVersion, "");

Console.Write("Remote Version:");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($" {latestVersion}");
Console.ForegroundColor = defaultColor;

var isValid = new Version(metaVersion) > new Version(latestVersion) && Version.IsValid(metaVersion) && Version.IsValid(latestVersion);

Console.ForegroundColor = isValid ? ConsoleColor.Green : ConsoleColor.Red;
Console.WriteLine(isValid ? "OK: Local Version is newer" : "ERROR: Local Version is behind Remote");
Console.ForegroundColor = defaultColor;