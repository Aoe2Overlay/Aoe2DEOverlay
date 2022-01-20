using System;
using System.IO.Compression;

string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));

var publicPath = $"{rootPath}\\publish\\Aoe2DEOverlay";
string x64Path = $"{publicPath}\\x64";
string x86Path = $"{publicPath}\\x86";
string x64ZipPath = $"{publicPath}\\x64.zip";
string x86ZipPath = $"{publicPath}\\x64.zip";

ZipFile.CreateFromDirectory(x64Path, x64ZipPath);
ZipFile.CreateFromDirectory(x86Path, x86ZipPath);