string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));

var secretFile = $"{rootPath}\\Aoe2DEOverlay\\AppInfo\\Secret.cs";

if(File.Exists(secretFile))
{
    Console.WriteLine("OK: Secret Exists");
}
else 
{
    Console.WriteLine("ERROR: Secret Exists");
}