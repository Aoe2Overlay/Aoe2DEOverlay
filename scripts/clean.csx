string GetCurrentScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null)
{
    return fileName;
}

var rootPath = String.Join('\\', GetCurrentScriptPath().Split('\\').SkipLast(2));

var publicPath = $"{rootPath}\\publish";

if(Directory.Exists(publicPath)) Directory.Delete(publicPath, true);